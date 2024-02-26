using CityInfo.API.Entities;
using CityInfo.API.Models;

namespace CityInfo.API
{
    public class CitiesDataStore
    {
        private object _lock = new object();

        public CityInfoContext Context { get; set; }

        public CitiesDataStore(CityInfoContext context)
        {
            Context = context;
        }

        public IEnumerable<CityDto> GetCities()
        {
            return Context.Cities.Select(c => new CityDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                PointsOfInterests = c.PointsOfInterests.Select(poi => new PointOfInterestDto
                {
                    Id = poi.Id,
                    Name = poi.Name,
                    Description = poi.Description
                }).ToList()
            }).ToList();
        }

        public CityDto? GetCity(int id)
        {
            var cityEntity = Context.Cities.FirstOrDefault(x => x.Id == id);
            if (cityEntity == null)
            {
                return null;
            }

            return new CityDto
            {
                Id = cityEntity.Id,
                Name = cityEntity.Name,
                Description = cityEntity.Description,
                PointsOfInterests = cityEntity.PointsOfInterests.Select(poi => new PointOfInterestDto
                {
                    Id = poi.Id,
                    Name = poi.Name,
                    Description = poi.Description
                }).ToList()
            };
        }

        public PointOfInterestDto CreatePointOfInterest(int cityId, PointOfInterestForCreationDto pointOfInterest)
        {
            var city = Context.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null)
            {
                throw new ArgumentException("City not found", nameof(cityId));
            }

            lock (_lock)
            {
                var poi = new PointOfInterest(pointOfInterest.Name) { Description = pointOfInterest.Description, City = city, CityId = city.Id };
                city.PointsOfInterests.Add(poi);
                Context.PointsOfInterest.Add(poi);

                Context.SaveChanges();

                return new PointOfInterestDto
                {
                    Id = poi.Id,
                    Name = poi.Name,
                    Description = poi.Description
                };
            }
        }
    }
}
