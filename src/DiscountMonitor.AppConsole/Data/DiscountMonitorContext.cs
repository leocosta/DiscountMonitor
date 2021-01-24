using Microsoft.EntityFrameworkCore;
using DiscountMonitor.AppConsole.Models;

namespace DiscountMonitor.AppConsole.Data
{
    public class DiscountMonitorContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<HistoryPrice> HistoryPrices { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseSqlite(@"Data Source=./Data/Db/DiscountMonitor.db;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>()
                .HasKey(u => u.Id);

            modelBuilder.Entity<Product>()
                .HasIndex(e => e.Url)
                .IsUnique();

            modelBuilder.Entity<Product>()
                .Ignore(e => e.PriceWasChanged);

            modelBuilder.Entity<HistoryPrice>()
                .HasKey(u => u.Id);

            modelBuilder.Entity<HistoryPrice>()
                .HasOne<Product>(i => i.Product)
                .WithMany(e => e.HistoryPrices)
                .IsRequired();
        }
    }
}
