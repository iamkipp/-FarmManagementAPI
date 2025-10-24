using Microsoft.EntityFrameworkCore;

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
        });

        // Farm configuration
        modelBuilder.Entity<Farm>(entity =>
        {
            entity.HasKey(f => f.Id);
            entity.HasOne(f => f.User)
                  .WithMany(u => u.Farms)
                  .HasForeignKey(f => f.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // FarmRecord configuration
        modelBuilder.Entity<FarmRecord>(entity =>
        {
            entity.HasKey(fr => fr.Id);
            entity.HasOne(fr => fr.Farm)
                  .WithMany(f => f.FarmRecords)
                  .HasForeignKey(fr => fr.FarmId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Subscription configuration
        modelBuilder.Entity<Subscription>(entity =>
        {
            entity.HasKey(s => s.Id);
            entity.HasOne(s => s.User)
                  .WithOne(u => u.Subscription)
                  .HasForeignKey<Subscription>(s => s.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
