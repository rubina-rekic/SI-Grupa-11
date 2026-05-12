using Microsoft.EntityFrameworkCore;
using PostRoute.DAL.Entities;

namespace PostRoute.DAL;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<SecurityLog> SecurityLogs => Set<SecurityLog>();
    public DbSet<Mailbox> Mailboxes => Set<Mailbox>();
    public DbSet<MailboxAuditLog> MailboxAuditLogs => Set<MailboxAuditLog>();
    public DbSet<Route> Routes => Set<Route>();
    public DbSet<RouteItem> RouteItems => Set<RouteItem>();

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

        modelBuilder.Entity<Mailbox>(entity =>
        {
            entity.HasKey(m => m.Id);
            entity.HasIndex(m => m.SerialNumber).IsUnique();
            entity.Property(m => m.SerialNumber).IsRequired().HasMaxLength(50);
            entity.Property(m => m.Address).IsRequired().HasMaxLength(200);
            entity.Property(m => m.Capacity).IsRequired();
            entity.Property(m => m.InstallationYear).IsRequired();
            entity.Property(m => m.Notes).HasMaxLength(500);
        });

        modelBuilder.Entity<MailboxAuditLog>(entity =>
        {
            entity.HasKey(m => m.Id);
            entity.Property(m => m.FieldName).IsRequired().HasMaxLength(50);
            entity.Property(m => m.Action).IsRequired().HasMaxLength(20);
            entity.HasOne(m => m.Mailbox)
                .WithMany()
                .HasForeignKey(m => m.MailboxId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(m => m.User)
                .WithMany()
                .HasForeignKey(m => m.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Route>(entity =>
        {
            entity.HasKey(r => r.Id);
            entity.Property(r => r.Date).HasColumnType("date");
            entity.Property(r => r.PlannedStartTime).HasColumnType("time");
            entity.Property(r => r.PlannedEndTime).HasColumnType("time");
            entity.HasOne(r => r.Postman)
                .WithMany()
                .HasForeignKey(r => r.PostmanId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<RouteItem>(entity =>
        {
            entity.HasKey(ri => ri.Id);
            entity.Property(ri => ri.EstimatedArrivalTime).HasColumnType("time");
            entity.HasOne(ri => ri.Route)
                .WithMany(r => r.RouteItems)
                .HasForeignKey(ri => ri.RouteId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(ri => ri.Mailbox)
                .WithMany()
                .HasForeignKey(ri => ri.MailboxId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
