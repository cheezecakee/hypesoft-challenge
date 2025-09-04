using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore.Extensions;
using Hypesoft.Domain.Entities;

namespace Hypesoft.Infrastructure.Data.Context
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure MongoDB collections
            modelBuilder.Entity<Product>().ToCollection("products");
            modelBuilder.Entity<Category>().ToCollection("categories");

            // Apply configurations
            ApplyConfigurations(modelBuilder);
        }

        private static void ApplyConfigurations(ModelBuilder modelBuilder)
        {
            // Product configuration
            modelBuilder.Entity<Product>(static entity =>
            {
                entity.HasKey(static p => p.Id);
                entity.Property(static p => p.Id).ValueGeneratedNever();
                entity.Property(static p => p.Name).IsRequired().HasMaxLength(200);
                entity.Property(static p => p.Description).IsRequired();
                entity.Property(static p => p.CategoryId).IsRequired();
                entity.Property(static p => p.StockQuantity).IsRequired();
                entity.Property(static p => p.CreateAt).IsRequired();
                entity.Property(static p => p.UpdatedAt);

                // Configure Money value object
                entity.OwnsOne(static p => p.Price, static money =>
                {
                    money.Property(static m => m.Amount).IsRequired();
                    money.Property(static m => m.Currency).IsRequired().HasMaxLength(3);
                });

                // Navigation - ignore for now (MongoDB EF Core has limited navigation support)
                entity.Ignore(static p => p.Category);
                entity.Ignore(static p => p.IsLowStock);
                entity.Ignore(static p => p.DomainEvents);

                // Indexes for better performance
                entity.HasIndex(static p => p.Name);
                entity.HasIndex(static p => p.CategoryId);
                entity.HasIndex(static p => p.StockQuantity);
            });

            // Category configuration
            modelBuilder.Entity<Category>(static entity =>
            {
                entity.HasKey(static c => c.Id);
                entity.Property(static c => c.Id).ValueGeneratedNever();
                entity.Property(static c => c.Name).IsRequired().HasMaxLength(100);
                entity.Property(static c => c.Description);
                entity.Property(static c => c.CreateAt).IsRequired();
                entity.Property(static c => c.UpdatedAt);

                // Navigation - ignore for now
                entity.Ignore(static c => c.Products);
                entity.Ignore(static c => c.DomainEvents);

                // Index for better performance
                entity.HasIndex(static c => c.Name).IsUnique();
            });
        }
    }
}
