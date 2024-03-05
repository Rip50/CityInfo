using AutoMapper;
using CityInfo.API.Entities;
using CityInfo.API.Models;
using Microsoft.EntityFrameworkCore;

namespace CityInfo.API
{
    public interface ICityInfoRepository
    {
        Task<IEnumerable<CityDto>> GetCities();
        Task<CityDto?> GetCity(int id, bool includePointsOfInterest = false);
        Task<PointOfInterestDto?> GetPointOfinterestForCity(int cityId, int pointOfInterestId);
        Task<PointOfInterestDto> CreatePointOfInterest(int cityId, PointOfInterestForCreationDto pointOfInterest);
        Task UpdatePointOfInterest(int cityId, int pointOfInterestId, PointOfInterestForUpdateDto pointOfInterest);
        Task DeletePointOfInterestForCity(int cityId, int pointOfInterestId);
        Task DeletePointOfInterest(int cityId, PointOfInterestDto poi);
    }


    public class CityInfoRepository : ICityInfoRepository
    {
        private CityInfoContext Context { get; set; }
        private readonly IMapper _mapper;  

        public CityInfoRepository(CityInfoContext context, IMapper mapper)
        {
            Context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CityDto>> GetCities()
        {
            return await Context.Cities
                .Include(c => c.PointsOfInterests)
                .Select(c => _mapper.Map<CityDto>(c))
                .ToListAsync();
        }

        public async Task<CityDto?> GetCity(int id, bool includePointsOfInterest = false)
        {
            if (includePointsOfInterest)
            {
                return _mapper.Map<CityDto>(await Context.Cities
                    .Include(c => c.PointsOfInterests)
                    .FirstOrDefaultAsync(x => x.Id == id));
            }

            return _mapper.Map<CityDto>(await Context.Cities
                .FirstOrDefaultAsync(x => x.Id == id));
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

            return _mapper.Map<PointOfInterestDto>(poi);
            
        }

        public async Task<PointOfInterestDto?> GetPointOfinterestForCity(int cityId, int pointOfInterestId)
        {
            var city = await GetCity(cityId, true);
            if (city == null)
            {
                throw new ArgumentException("City not found", nameof(cityId));
            }

            var poi = city.PointsOfInterests.FirstOrDefault(p => p.Id == pointOfInterestId);
            if (poi == null)
            {
                throw new ArgumentException("Point of interest not found", nameof(pointOfInterestId));
            }

            return _mapper.Map<PointOfInterestDto>(poi);
        }

        public async Task UpdatePointOfInterest(int cityId, int pointOfInterestId, PointOfInterestForUpdateDto pointOfInterest)
        {
            var poiEntity = await Context.PointsOfInterest
                .Where(p => p.Id == pointOfInterestId && p.CityId == cityId)
                .FirstOrDefaultAsync();

            if (poiEntity == null)
            {
                throw new ArgumentException("Point of interest not found", nameof(pointOfInterestId));
            }

            _mapper.Map(pointOfInterest, poiEntity);

            await Context.SaveChangesAsync();
        }

        public async Task DeletePointOfInterestForCity(int cityId, int pointOfInterestId)
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
            city.PointsOfInterests.Remove(poi);
            
            await Context.SaveChangesAsync();
        }


        // Not working properly
        public async Task DeletePointOfInterest(int cityId, PointOfInterestDto poi)
        {
            var city = await Context.Cities.Include(c => c.PointsOfInterests)
                .FirstOrDefaultAsync(c => c.Id == cityId);
            if (city == null)
            {
                throw new ArgumentException("City not found", nameof(cityId));
            }
            city.PointsOfInterests.Remove(_mapper.Map<PointOfInterest>(poi));

            await Context.SaveChangesAsync();
        }
    }
}