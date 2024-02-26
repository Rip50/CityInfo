using CityInfo.API.Entities;
using CityInfo.API.Models;
using Microsoft.EntityFrameworkCore;

namespace CityInfo.API
{
    public interface ICityInfoRepository
    {
        Task<IEnumerable<CityDto>> GetCities();
        Task<CityDto?> GetCity(int id);
        Task<PointOfInterestDto?> GetPointOfinterestForCity(int cityId, int pointOfInterestId);
        Task<PointOfInterestDto> CreatePointOfInterest(int cityId, PointOfInterestForCreationDto pointOfInterest);
        Task UpdatePointOfInterest(int cityId, int pointOfInterestId, PointOfInterestForUpdateDto pointOfInterest);
    }


    public class CityInfoRepository : ICityInfoRepository
    {
        public CityInfoContext Context { get; set; }

        public CityInfoRepository(CityInfoContext context)
        {
            Context = context;
        }

        public async Task<IEnumerable<CityDto>> GetCities()
        {
            return await Context.Cities
                .Include(c => c.PointsOfInterests)
                .Select(c => new CityDto
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
            }).ToListAsync();
        }

        public async Task<CityDto?> GetCity(int id)
        {
            var cityEntity = await Context.Cities
                .Include(c => c.PointsOfInterests)
                .FirstOrDefaultAsync(x => x.Id == id);
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

        public async Task<PointOfInterestDto> CreatePointOfInterest(int cityId, PointOfInterestForCreationDto pointOfInterest)
        {
            var city = await Context.Cities.FirstOrDefaultAsync(c => c.Id == cityId);
            if (city == null)
            {
                throw new ArgumentException("City not found", nameof(cityId));
            }

            var poi = new PointOfInterest(pointOfInterest.Name) { Description = pointOfInterest.Description, City = city, CityId = city.Id };
            city.PointsOfInterests.Add(poi);
            Context.PointsOfInterest.Add(poi);

            await Context.SaveChangesAsync();

            return new PointOfInterestDto
            {
                Id = poi.Id,
                Name = poi.Name,
                Description = poi.Description
            };
            
        }

        public async Task<PointOfInterestDto?> GetPointOfinterestForCity(int cityId, int pointOfInterestId)
        {
            var city = await Context.Cities.FirstOrDefaultAsync(c => c.Id == cityId);
            if (city == null)
            {
                throw new ArgumentException("City not found", nameof(cityId));
            }

            var poi = city.PointsOfInterests.FirstOrDefault(p => p.Id == pointOfInterestId);
            if (poi == null)
            {
                throw new ArgumentException("Point of interest not found", nameof(pointOfInterestId));
            }

            return new PointOfInterestDto
            {
                Id = poi.Id,
                Name = poi.Name,
                Description = poi.Description
            };
        }

        public async Task UpdatePointOfInterest(int cityId, int pointOfInterestId, PointOfInterestForUpdateDto pointOfInterest)
        {
            var city = await Context.Cities.Include(c => c.PointsOfInterests)
                .FirstOrDefaultAsync(c => c.Id == cityId);
            if (city == null)
            {
                throw new ArgumentException("City not found", nameof(cityId));
            }

            var poi = city.PointsOfInterests.FirstOrDefault(p => p.Id == pointOfInterestId);
            if (poi == null)
            {
                throw new ArgumentException("Point of interest not found", nameof(pointOfInterestId));
            }

            poi.Name = pointOfInterest.Name;
            poi.Description = pointOfInterest.Description;
            await Context.SaveChangesAsync();
        }
    }
}