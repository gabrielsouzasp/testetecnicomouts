using Microsoft.EntityFrameworkCore;
using Mouts.SalesRecords.Domain.Entities;

namespace Mouts.SalesRecords.Infra.Database
{
    public class SalesContext : DbContext
    {
        public DbSet<Sale> Sales { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<SaleProduct> SaleProducts { get; set; }

        public SalesContext(DbContextOptions<SalesContext> options)
        : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<SaleProduct>(entity =>
            {
                entity.HasKey(sp => sp.Id);

                entity.HasOne(sp => sp.Sale);

                entity.HasOne(sp => sp.Product)
                    .WithMany() 
                    .HasForeignKey(sp => sp.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.Property(sp => sp.Quantity)
                    .IsRequired();

                entity.Property(sp => sp.UnitPrice)
                    .IsRequired();
            });

            modelBuilder.Entity<Sale>(entity =>
            {
                entity.HasKey(s => s.Id);
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(p => p.Id);
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(c => c.Id);
            });

            modelBuilder.Entity<Branch>(entity =>
            {
                entity.HasKey(b => b.Id);
            });
        }
    }
}
