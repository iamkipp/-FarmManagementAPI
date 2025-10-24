using Microsoft.EntityFrameworkCore;
using FarmManagement.Core.Entities;

namespace FarmManagement.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

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

            // One-to-one relationship with Subscription
            entity.HasOne(u => u.Subscription)
                  .WithOne(s => s.User)
                  .HasForeignKey<Subscription>(s => s.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Farm configuration
        modelBuilder.Entity<Farm>(entity =>
        {
            entity.HasKey(f => f.Id);
            entity.Property(f => f.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

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

            entity.HasIndex(s => s.PendingCheckoutRequestId).IsUnique().HasFilter("[PendingCheckoutRequestId] IS NOT NULL");
        });
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Add audit trail logic here if needed in the future
        return await base.SaveChangesAsync(cancellationToken);
    }
}