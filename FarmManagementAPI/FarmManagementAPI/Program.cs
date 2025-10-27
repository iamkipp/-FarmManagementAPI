using FarmManagement.Core.Interfaces;
using FarmManagementAPI.FarmManagement.Infrastructure.Data;
using FarmManagement.Infrastructure.Repositories;
using FarmManagementAPI.FarmManagement.Infrastructure.Services;
using FarmManagementAPI.FarmManagement.Core.Entities;
using FarmManagementAPI.FarmManagement.Core.Interfaces.IServices;
using FarmManagementAPI.FarmManagement.Core.Interfaces.Repositories;
using FarmManagementAPI.FarmManagement.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using FarmManagement.Infrastructure.Services;


var builder = WebApplication.CreateBuilder(args);

// ========== MANUAL CONFIGURATION LOAD ==========
Console.WriteLine("🔧 Loading configuration...");

// Get the connection string
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("❌ Connection string 'DefaultConnection' is null or empty. Check your appsettings.json file.");
}

Console.WriteLine($"✅ Connection string loaded successfully!");
Console.WriteLine($"📁 Database: FarmManagement");
Console.WriteLine($"🔗 Server: KIPP_01\\SQLEXPRESS");

// ========== SERVICES CONFIGURATION ==========
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

//// Scalar configuration
//builder.Services.AddScalar(builder =>
//{
//    builder.WithTitle("Farm Management API")
//           .WithDescription("Farm management platform with M-Pesa integration")
//           .WithDefaultHttpClient(ScalarConstants.HttpClientMode.Fetch);
//});

// ✅ FIXED: Database Configuration - Use the verified connection string
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

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

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactAndScalar", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "https://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// ========== REPOSITORIES & SERVICES ==========
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IFarmRepository, FarmRepository>();
builder.Services.AddScoped<IFarmRecordRepository, FarmRecordRepository>();
builder.Services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();


builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPaymentService, MpesaPaymentService>();
builder.Services.AddScoped<ISubscriptionService, SubscriptionService>();
builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();

builder.Services.AddHttpClient();

var app = builder.Build();

// ========== DATABASE MIGRATION ==========
using (var scope = app.Services.CreateScope())
{
    try
    {
        Console.WriteLine("🔄 Starting database migration...");

        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Test connection first
        var canConnect = await dbContext.Database.CanConnectAsync();
        Console.WriteLine($"📊 Database connection: {(canConnect ? "✅ SUCCESS" : "❌ FAILED")}");

        if (canConnect)
        {
            // Get migration status
            var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();
            var appliedMigrations = await dbContext.Database.GetAppliedMigrationsAsync();

            Console.WriteLine($"📦 Applied migrations: {appliedMigrations.Count()}");
            Console.WriteLine($"⏳ Pending migrations: {pendingMigrations.Count()}");

            if (pendingMigrations.Any())
            {
                Console.WriteLine("🔄 Applying migrations...");
                await dbContext.Database.MigrateAsync();
                Console.WriteLine("✅ Database migrated successfully!");
            }
            else
            {
                Console.WriteLine("✅ Database is up to date!");
            }

            // Seed initial admin user
            await SeedAdminUser(dbContext, builder.Configuration);
        }
        else
        {
            Console.WriteLine("❌ Cannot connect to database. Please check:");
            Console.WriteLine("   1. SQL Server (SQLEXPRESS) is running");
            Console.WriteLine("   2. Instance name: KIPP_01\\SQLEXPRESS");
            Console.WriteLine("   3. Windows Authentication is enabled");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Database migration failed: {ex.Message}");
        // Don't crash the app - just log the error
    }
}

// ========== MIDDLEWARE PIPELINE ==========
if (app.Environment.IsDevelopment())
{
    //app.UseScalar(settings =>
    //{
    //    settings.WithTheme(ScalarTheme.Moon)
    //           .WithDefaultHttpClient(ScalarConstants.HttpClientMode.Fetch);
    //});
}

app.UseHttpsRedirection();
app.UseCors("ReactAndScalar");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Health checks
app.MapGet("/", () => Results.Redirect("/scalar"));
app.MapGet("/health", () => Results.Ok(new
{
    status = "Healthy",
    timestamp = DateTime.UtcNow,
    database = "FarmManagement",
    environment = app.Environment.EnvironmentName
}));

Console.WriteLine($"🚀 Farm Management API starting...");
Console.WriteLine($"📍 Environment: {app.Environment.EnvironmentName}");
Console.WriteLine($"📚 API Documentation: https://localhost:7115/scalar");
Console.WriteLine($"❤️  Health Check: https://localhost:7115/health");

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

        adminUser.Subscription = new Subscription
        {
            Id = Guid.NewGuid(),
            UserId = adminUser.Id,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddYears(10),
            IsActive = true,
            IsTrial = false,
            Status = "Active"
        };

        await context.Users.AddAsync(adminUser);
        await context.SaveChangesAsync();

        Console.WriteLine("✅ Admin user seeded successfully!");
    }
}