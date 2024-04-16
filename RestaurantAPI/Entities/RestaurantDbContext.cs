using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using RestaurantAPI.Data;

namespace RestaurantAPI.Entities
{
    public class RestaurantDbContext : DbContext
    {
        private readonly string _connectionString = "Server=.\\SQLEXPRESS;Database=RestaurantDb;Trusted_Connection=True;TrustServerCertificate=True;";

        public DbSet<Restaurant> Restaurants { get; set;}
        public DbSet<Address> Addresses { get; set;}
        public DbSet<Dish> Dishes { get; set;}

        // Connection to the data base or additional config

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Restaurant>()
                .Property(r => r.Name)
                .IsRequired()
                .HasMaxLength(25);

            modelBuilder.Entity<Dish>()
                .Property(d => d.Name)
                .IsRequired();

            modelBuilder.Entity<Address>()
                .Property(a => a.Street)
                .IsRequired()
                .HasMaxLength(50);
            
            modelBuilder.Entity<Address>()
                .Property(a => a.City)
                .IsRequired()
                .HasMaxLength(50);

            // Data Seed - but we use RestaurantSeeder
            //modelBuilder.Entity<Restaurant>()
            //    .HasData();
            //new RestaurantSeeder(this).Seed();    // This way will add only when model is updating to the db
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer(_connectionString);
        }
    }
}
