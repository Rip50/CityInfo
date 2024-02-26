using CityInfo.API.Models;

namespace CityInfo.API
{
    public class CitiesDataStore
    {
        private object _lock = new object();

        public List<CityDto> Cities { get; set; }

        public static CitiesDataStore Current { get; set; } = new CitiesDataStore();

        private CitiesDataStore()
        {
            Cities = new List<CityDto>()
            {
                new CityDto() { Id = 1, Name="Malaga", Description="Great city",
                    PointsOfInterests =
                    {
                        new PointOfInterestDto() { Id = 1, Name = "Name_1", Description = "Description_1"},
                        new PointOfInterestDto() { Id = 2, Name = "Name_2", Description = "Description_2"},
                    } 
                },
                new CityDto() { Id = 2, Name="Seville", Description="Where I'm going to go",
                    PointsOfInterests =
                    {
                        new PointOfInterestDto() { Id = 3, Name = "Name_3", Description = "Description_3"},
                        new PointOfInterestDto() { Id = 4, Name = "Name_4", Description = "Description_4"},
                    } 
                },
                new CityDto() { Id = 3, Name="NYC", Description="I was there",
                    PointsOfInterests =
                    {
                        new PointOfInterestDto() { Id = 5, Name = "Name_5", Description = "Description_5"},
                        new PointOfInterestDto() { Id = 6, Name = "Name_6", Description = "Description_6"},
                    }
                }
            };
        }

        public PointOfInterestDto CreatePointOfInterest(int cityId, PointOfInterestForCreationDto pointOfInterest)
        {
            var city = Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null)
            {
                throw new ArgumentException("City not found", nameof(cityId));
            }

            lock (_lock)
            {
                var lastId = Cities.Max(c => c.PointsOfInterests.Max(pointOfInterest => pointOfInterest.Id));

                var newId = lastId + 1;
                var poi = new PointOfInterestDto { Id = newId, Name = pointOfInterest.Name, Description = pointOfInterest.Description };
                city.PointsOfInterests.Add(poi);
                return poi;
            }
        }
    }
}
