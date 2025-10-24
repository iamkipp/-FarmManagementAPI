using FarmManagement.Core.Interfaces;
using FarmManagement.Infrastructure.Data;
using FarmManagement.Infrastructure.Repositories;
using FarmManagement.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Safaricom.Daraja; // Or your chosen M-Pesa SDK namespace
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Add Scalar instead of Swagger
builder.Services.AddScalar(options =>
{
    options.WithTitle("Farm Management API")
           .WithDescription("A comprehensive farm management platform with M-Pesa integration")
           .WithDefaultHttpClient(ScalarConstants.HttpClientMode.Fetch);
});

// Database Configuration
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(builder.Configuration["Jwt:Key"]!)),
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// CORS - Configure for React frontend and Scalar
builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactAndScalar", policy =>
    {
        policy.WithOrigins(
                "http://localhost:3000",  // React dev server
                "https://localhost:3000",
                "http://localhost:3001",
                "https://localhost:3001",
                "https://cdn.jsdelivr.net"  // Scalar CDN
              )
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// ========== REPOSITORIES ==========
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IFarmRepository, FarmRepository>();
builder.Services.AddScoped<IFarmRecordRepository, FarmRecordRepository>();

// ========== SERVICES ==========
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPaymentService, MpesaPaymentService>();
builder.Services.AddScoped<ISubscriptionService, SubscriptionService>();
builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();

// ========== M-PESA CONFIGURATION ==========
builder.Services.Configure<MpesaConfiguration>(options =>
{
    options.ConsumerKey = builder.Configuration["Mpesa:ConsumerKey"]!;
    options.ConsumerSecret = builder.Configuration["Mpesa:ConsumerSecret"]!;
    options.BusinessShortCode = builder.Configuration["Mpesa:BusinessShortCode"]!;
    options.Passkey = builder.Configuration["Mpesa:Passkey"]!;
    options.Environment = builder.Configuration["Mpesa:Environment"] ?? "Sandbox";
});

builder.Services.AddScoped<IMpesaClient, MpesaClient>();

// ========== HTTP CLIENT for M-PESA API ==========
builder.Services.AddHttpClient("Mpesa", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Mpesa:BaseUrl"]
        ?? "https://sandbox.safaricom.co.ke/");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

// ========== BACKGROUND SERVICES ==========
builder.Services.AddHostedService<SubscriptionBackgroundService>();

// ========== LOGGING ==========
builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.AddDebug();
    logging.AddConfiguration(builder.Configuration.GetSection("Logging"));
});

var app = builder.Build();

// ========== DATABASE MIGRATION ==========
using (var scope = app.Services.CreateScope())
{
    try
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        dbContext.Database.Migrate();

        // Seed initial admin user if needed
        await SeedAdminUser(dbContext, builder.Configuration);

        Console.WriteLine("Database migrated successfully!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Database migration failed: {ex.Message}");
    }
}

// ========== CONFIGURE PIPELINE ==========
if (app.Environment.IsDevelopment())
{
    // Scalar API Reference - Much cleaner than Swagger UI
    app.UseScalarApiReference(settings =>
    {
        settings.WithTheme(ScalarTheme.Moon)
                .WithDefaultHttpClient(ScalarConstants.HttpClientMode.Fetch)
                .WithSidebar(true)
                .WithSearchHotKey("k")
                .WithHideDownloadButton();
    });

    // Development logging
    app.Use(async (context, next) =>
    {
        Console.WriteLine($"Request: {context.Request.Method} {context.Request.Path}");
        await next();
    });
}

app.UseHttpsRedirection();

// CORS must come before Authentication
app.UseCors("ReactAndScalar");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Health check endpoint
app.MapGet("/", () => Results.Redirect("/scalar"));
app.MapGet("/health", () => Results.Ok(new
{
    status = "Healthy",
    timestamp = DateTime.UtcNow,
    service = "Farm Management API",
    version = "1.0.0"
}));

// API info endpoint
app.MapGet("/api/info", () => new
{
    Name = "Farm Management API",
    Version = "1.0.0",
    Description = "Farm management platform with M-Pesa integration",
    Environment = app.Environment.EnvironmentName,
    Documentation = "/scalar",
    Health = "/health"
});

// Global exception handling
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An unhandled exception occurred.");

        context.Response.StatusCode = 500;
        await context.Response.WriteAsJsonAsync(new
        {
            error = "An internal server error occurred",
            message = app.Environment.IsDevelopment() ? ex.Message : "Contact support"
        });
    }
});

Console.WriteLine($"Farm Management API starting...");
Console.WriteLine($"Environment: {app.Environment.EnvironmentName}");
Console.WriteLine($"M-Pesa Environment: {builder.Configuration["Mpesa:Environment"]}");
Console.WriteLine($"API Documentation: https://localhost:7000/scalar");

app.Run();

// ========== HELPER METHODS ==========
async Task SeedAdminUser(ApplicationDbContext context, IConfiguration config)
{
    if (!await context.Users.AnyAsync(u => u.Role == "Admin"))
    {
        using var hmac = new HMACSHA512();

        var adminUser = new User
        {
            Id = Guid.NewGuid(),
            Email = config["Admin:Email"] ?? "admin@farmmanagement.com",
            FirstName = "System",
            LastName = "Admin",
            PhoneNumber = config["Admin:Phone"] ?? "254700000000",
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(
                config["Admin:Password"] ?? "Admin123!")),
            PasswordSalt = hmac.Key,
            Role = "Admin",
            CreatedAt = DateTime.UtcNow
        };

        // Admin doesn't need a subscription
        adminUser.Subscription = new Subscription
        {
            Id = Guid.NewGuid(),
            UserId = adminUser.Id,
            StartDate = DateTime.UtcNow,
            IsActive = true,
            IsTrial = false,
            Status = "Active"
        };

        await context.Users.AddAsync(adminUser);
        await context.SaveChangesAsync();

        Console.WriteLine("Admin user seeded successfully!");
    }
}