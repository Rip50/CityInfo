using Microsoft.EntityFrameworkCore;

namespace CityInfo.API.Entities
{
    public class CityInfoContext : DbContext
    {
        public CityInfoContext(DbContextOptions<CityInfoContext> options)
            : base(options)
        {
            // Causes issues with migrations
            //Database.EnsureCreated();

            //Cities.Include(c => c.PointsOfInterests);
        }

        public DbSet<City> Cities { get; set; } = null!;
        public DbSet<PointOfInterest> PointsOfInterest { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<City>().HasData(
                new City("New York City")
                {
                    Id = 1,
                    Description = "The one with that big park."
                },
                new City("Antwerp")
                {
                    Id = 2,
                    Description = "The one with the cathedral that was never really finished."
                },
                new City("Paris")
                {
                    Id = 3,
                    Description = "The one with that big tower."
                });

            modelBuilder.Entity<PointOfInterest>().HasData(
                new PointOfInterest("Central Park")
                {
                    Id = 1,
                    CityId = 1,
                    Description = "The most visited urban park in the United States."
                }, 
                new PointOfInterest("Empire State Building")
                {
                    Id = 2,
                    CityId = 1,
                    Description = "A 102-story skyscraper located in Midtown Manhattan."
                }, 
                new PointOfInterest("Name_3")
                {
                    Id = 3,
                    CityId = 2,
                    Description = "Description_3"
                },
                new PointOfInterest("Name_4")
                {
                    Id = 4,
                    CityId = 2,
                    Description = "Description_4"
                },
                new PointOfInterest("Name_5")
                {
                    Id = 5,
                    CityId = 3,
                    Description = "Description_5"
                },
                new PointOfInterest("Name_6")
                {
                    Id = 6,
                    CityId = 3,
                    Description = "Description_6"
                });

            base.OnModelCreating(modelBuilder);
        }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
            //optionsBuilder.UseSqlite("Data Source=CityInfo.db");
            //base.OnConfiguring(optionsBuilder);
        //}
    }
}
