using FarmManagementAPI.FarmManagement.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace FarmManagementAPI.FarmManagement.Infrastructure.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Farm> Farms { get; set; }
    public DbSet<FarmRecord> FarmRecords { get; set; }
    public DbSet<Subscription> Subscriptions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.HasIndex(u => u.Email).IsUnique();
            entity.Property(u => u.Role).HasDefaultValue("Farmer");
            entity.Property(u => u.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
        });

        // Farm configuration
        modelBuilder.Entity<Farm>(entity =>
        {
            entity.HasKey(f => f.Id);
            entity.Property(f => f.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

            // Add decimal precision for SizeInAcres
            entity.Property(f => f.SizeInAcres)
                  .HasPrecision(18, 2); // 18 total digits, 2 decimal places

            entity.HasOne(f => f.User)
                  .WithMany(u => u.Farms)
                  .HasForeignKey(f => f.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // FarmRecord configuration
        modelBuilder.Entity<FarmRecord>(entity =>
        {
            entity.HasKey(fr => fr.Id);
            entity.Property(fr => fr.RecordDate).HasDefaultValueSql("GETUTCDATE()");

            // add decimal precision for all decimal properties
            entity.Property(fr => fr.SeedAmountInKg)
                  .HasPrecision(18, 2);

            entity.Property(fr => fr.PesticideAmountInLitres)
                  .HasPrecision(18, 2);

            entity.Property(fr => fr.WaterConsumptionInLitres)
                  .HasPrecision(18, 2);

            entity.Property(fr => fr.YieldInKg)
                  .HasPrecision(18, 2);

            entity.Property(fr => fr.Revenue)
                  .HasPrecision(18, 2);

            entity.HasOne(fr => fr.Farm)
                  .WithMany(f => f.FarmRecords)
                  .HasForeignKey(fr => fr.FarmId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Subscription configuration
        modelBuilder.Entity<Subscription>(entity =>
        {
            entity.HasKey(s => s.Id);
            entity.Property(s => s.StartDate).HasDefaultValueSql("GETUTCDATE()");
            entity.Property(s => s.IsActive).HasDefaultValue(true);
            entity.Property(s => s.IsTrial).HasDefaultValue(true);
            entity.Property(s => s.Status).HasDefaultValue("Active");

            //  Add decimal precision for LastPaymentAmount
            entity.Property(s => s.LastPaymentAmount)
                  .HasPrecision(18, 2);

            entity.HasIndex(s => s.PendingCheckoutRequestId)
                  .IsUnique()
                  .HasFilter("[PendingCheckoutRequestId] IS NOT NULL");
        });
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await base.SaveChangesAsync(cancellationToken);
    }
}