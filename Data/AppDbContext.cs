using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using main.Models;

namespace main.Data
{
    public class AppDbContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Property> Properties { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<PropertyImage> PropertyImages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Properties Casting
            builder.Entity<Booking>()
                .Property(b => b.CheckInDate)
                .HasColumnType("timestamp without time zone");
            
            builder.Entity<Booking>()
                .Property(b => b.CheckOutDate)
                .HasColumnType("timestamp without time zone");

            // Enums conversion to string
            builder.Entity<User>()
                .Property(u => u.Role)
                .HasConversion<string>();

            builder.Entity<Booking>()
                .Property(b => b.State)
                .HasConversion<string>();

            builder.Entity<Notification>()
                .Property(n => n.Type)
                .HasConversion<string>();

            // Default values
            builder.Entity<Property>()
                .Property(p => p.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
            
            builder.Entity<Property>()
                .Property(p => p.IsAvailable)
                .HasDefaultValue(true);

            builder.Entity<Booking>()
                .Property(b => b.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.Entity<Booking>()
                .Property(b => b.Paid)
                .HasDefaultValue(false);

            builder.Entity<Notification>()
                .Property(n => n.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
                
            builder.Entity<Notification>()
                .Property(n => n.IsRead)
                .HasDefaultValue(false);

            // Relationships Configuration

            // Property -> Owner (User)
            builder.Entity<Property>()
                .HasOne(p => p.Owner)
                .WithMany(u => u.Properties)
                .HasForeignKey(p => p.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Booking -> Guest (User)
            builder.Entity<Booking>()
                .HasOne(b => b.Guest)
                .WithMany(u => u.Bookings)
                .HasForeignKey(b => b.GuestId)
                .OnDelete(DeleteBehavior.Restrict);

            // Booking -> Property
            builder.Entity<Booking>()
                .HasOne(b => b.Property)
                .WithMany(p => p.Bookings)
                .HasForeignKey(b => b.PropertyId)
                .OnDelete(DeleteBehavior.Restrict);

            // Favorite -> User
            builder.Entity<Favorite>()
                .HasOne(f => f.User)
                .WithMany(u => u.Favorites)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Favorite -> Property
            builder.Entity<Favorite>()
                .HasOne(f => f.Property)
                .WithMany(p => p.Favorites)
                .HasForeignKey(f => f.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);

            // Notification -> User
            builder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // PropertyImage -> Property
            builder.Entity<PropertyImage>()
                .HasOne(pi => pi.Property)
                .WithMany(p => p.Images)
                .HasForeignKey(pi => pi.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
