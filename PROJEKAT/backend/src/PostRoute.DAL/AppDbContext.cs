using Microsoft.EntityFrameworkCore;
using PostRoute.DAL.Entities;

namespace PostRoute.DAL;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<SecurityLog> SecurityLogs => Set<SecurityLog>();
    public DbSet<Box> Boxes => Set<Box>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.HasIndex(u => u.Email).IsUnique();
            entity.HasIndex(u => u.Username).IsUnique();
            entity.Property(u => u.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(u => u.LastName).IsRequired().HasMaxLength(100);
            entity.Property(u => u.Username).IsRequired().HasMaxLength(50);
            entity.Property(u => u.Email).IsRequired().HasMaxLength(200);
            entity.Property(u => u.PasswordHash).IsRequired();
            entity.Property(u => u.Role).IsRequired().HasMaxLength(50);
        });

        modelBuilder.Entity<SecurityLog>(entity =>
        {
            entity.HasKey(s => s.Id);
            entity.Property(s => s.AttemptedUrl).IsRequired().HasMaxLength(500);
            entity.Property(s => s.AccessType).IsRequired().HasMaxLength(50);
            entity.Property(s => s.IpAddress).HasMaxLength(45);
            entity.Property(s => s.UserAgent).HasMaxLength(500);
        });

        modelBuilder.Entity<Box>(entity =>
        {
            entity.HasKey(b => b.Id);
            entity.HasIndex(b => b.SerialNumber).IsUnique();
            entity.Property(b => b.Address).IsRequired().HasMaxLength(255);
            entity.Property(b => b.Latitude).IsRequired().HasPrecision(10, 6);
            entity.Property(b => b.Longitude).IsRequired().HasPrecision(10, 6);
            entity.Property(b => b.Type).IsRequired().HasMaxLength(100);
            entity.Property(b => b.SerialNumber).IsRequired().HasMaxLength(50);
            entity.Property(b => b.Capacity).IsRequired();
            entity.Property(b => b.YearOfInstallation).IsRequired();
        });
    }
}