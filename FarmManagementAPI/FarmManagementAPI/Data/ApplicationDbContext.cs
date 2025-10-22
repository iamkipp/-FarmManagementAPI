using FarmManagement.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FarmManagement.API.Data
{
    public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Input> Inputs { get; set; }
        public DbSet<Output> Outputs { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure relationships
            builder.Entity<Input>()
                .HasOne(i => i.Farmer)
                .WithMany(u => u.Inputs)
                .HasForeignKey(i => i.FarmerId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Output>()
                .HasOne(o => o.Farmer)
                .WithMany(u => u.Outputs)
                .HasForeignKey(o => o.FarmerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Seed data
            SeedData(builder);
        }

        private void SeedData(ModelBuilder builder)
        {
            // Create roles
            var adminRoleId = Guid.NewGuid();
            var farmerRoleId = Guid.NewGuid();

            builder.Entity<IdentityRole<Guid>>().HasData(
                new IdentityRole<Guid> { Id = adminRoleId, Name = "Admin", NormalizedName = "ADMIN" },
                new IdentityRole<Guid> { Id = farmerRoleId, Name = "Farmer", NormalizedName = "FARMER" }
            );

            // Create admin user
            var adminId = Guid.NewGuid();
            var hasher = new PasswordHasher<User>();
            var adminUser = new User
            {
                Id = adminId,
                UserName = "admin@farm.com",
                NormalizedUserName = "ADMIN@FARM.COM",
                Email = "admin@farm.com",
                NormalizedEmail = "ADMIN@FARM.COM",
                Name = "System Administrator",
                Role = "Admin",
                DateCreated = DateTime.UtcNow,
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString()
            };
            adminUser.PasswordHash = hasher.HashPassword(adminUser, "Admin123!");

            builder.Entity<User>().HasData(adminUser);

            // Assign admin role to admin user
            builder.Entity<IdentityUserRole<Guid>>().HasData(
                new IdentityUserRole<Guid> { UserId = adminId, RoleId = adminRoleId }
            );

            // Create sample farmer
            var farmerId = Guid.NewGuid();
            var farmerUser = new User
            {
                Id = farmerId,
                UserName = "john@farm.com",
                NormalizedUserName = "JOHN@FARM.COM",
                Email = "john@farm.com",
                NormalizedEmail = "JOHN@FARM.COM",
                Name = "John Farmer",
                Role = "Farmer",
                DateCreated = DateTime.UtcNow,
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString()
            };
            farmerUser.PasswordHash = hasher.HashPassword(farmerUser, "Farmer123!");

            builder.Entity<User>().HasData(farmerUser);

            // Assign farmer role to farmer user
            builder.Entity<IdentityUserRole<Guid>>().HasData(
                new IdentityUserRole<Guid> { UserId = farmerId, RoleId = farmerRoleId }
            );

            // Seed sample inputs
            builder.Entity<Input>().HasData(
                new Input
                {
                    Id = Guid.NewGuid(),
                    FarmerId = farmerId,
                    InputType = "Seed",
                    Name = "Corn Seeds",
                    Quantity = 50,
                    Cost = 500,
                    DateAdded = DateTime.UtcNow.AddDays(-30)
                },
                new Input
                {
                    Id = Guid.NewGuid(),
                    FarmerId = farmerId,
                    InputType = "Fertilizer",
                    Name = "Organic Fertilizer",
                    Quantity = 100,
                    Cost = 300,
                    DateAdded = DateTime.UtcNow.AddDays(-20)
                }
            );

            // Seed sample outputs
            builder.Entity<Output>().HasData(
                new Output
                {
                    Id = Guid.NewGuid(),
                    FarmerId = farmerId,
                    CropType = "Corn",
                    QuantityHarvested = 1000,
                    Revenue = 2500,
                    DateRecorded = DateTime.UtcNow.AddDays(-5)
                }
            );
        }
    }
}