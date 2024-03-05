using AutoMapper;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [Route("api/cities/{cityId}/pointsofinterest")]
    [ApiController]
    [Authorize]
    public class PointsOfInterestController : ControllerBase
    {
        private ILogger<PointsOfInterestController> _logger;
        private ILocalMailService _mailService;
        private ICityInfoRepository _cityInfoRepositiory;
        private readonly IMapper _mapper;

        public PointsOfInterestController(ICityInfoRepository dataStore, ILogger<PointsOfInterestController> logger, 
            ILocalMailService mailService, IMapper mapper)
        {
            _logger = logger;
            _mailService = mailService;
            _cityInfoRepositiory = dataStore;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetPointsOfInterest(int cityId)
        {
            var city = await _cityInfoRepositiory.GetCity(cityId);
            if (city == null)
            {
                return NotFound();
            }
            
            return Ok(city.PointsOfInterests);
        }

        [HttpGet("{pointOfInterestId}", Name = "GetPointOfInterest")]
        public async Task<IActionResult> GetPointOfInterest(int cityId, int pointOfInterestId)
        {
            var city = await _cityInfoRepositiory.GetCity(cityId);
            if (city == null)
            {
                _logger.LogInformation($"City {cityId} was not found");
                return NotFound();
            }

            var pointOfInterest = city.PointsOfInterests.FirstOrDefault(x => x.Id == pointOfInterestId);
            if(pointOfInterest == null)
            {
                return NotFound();
            }

            return Ok(pointOfInterest);
        }

        [HttpPost]
        public async Task<IActionResult> AddPointOfInterest(int cityId, [FromBody] PointOfInterestForCreationDto pointOfInterest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var city = await _cityInfoRepositiory.GetCity(cityId);

            if (city == null)
            {
                return NotFound();
            }

            var poi = await _cityInfoRepositiory.CreatePointOfInterest(cityId, pointOfInterest);
            
            return CreatedAtRoute("GetPointOfInterest", new
            {
                cityId = cityId,
                pointOfInterestId = poi.Id
            }, poi);
        }


        [HttpPut("{pointOfInterestId}")]
        public async Task<IActionResult> UpdatePointOfInterest(int cityId, int pointOfInterestId, [FromBody] PointOfInterestForUpdateDto update)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            try
            {
                await _cityInfoRepositiory.UpdatePointOfInterest(cityId, pointOfInterestId, update);
            }
            catch (ArgumentException e)
            {
                return NotFound(e.Message);
            }

            return NoContent();
        }

        [HttpPatch("{pointOfInterestId}")]
        public async Task<IActionResult> PartiallyUpdatePointOfInterest(int cityId, int pointOfInterestId, 
            JsonPatchDocument<PointOfInterestForUpdateDto> patchDocument)
        {
            try
            {
                var poi = await _cityInfoRepositiory.GetPointOfinterestForCity(cityId, pointOfInterestId);
                if (poi == null)
                {
                    return NotFound();
                }

                var update = _mapper.Map<PointOfInterestForUpdateDto>(poi);
                patchDocument.ApplyTo(update, ModelState);

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                if (!TryValidateModel(update))
                {
                    return BadRequest(ModelState);
                }

                await _cityInfoRepositiory.UpdatePointOfInterest(cityId, pointOfInterestId, update);
            }
            catch (ArgumentException e)
            {
                return NotFound(e.Message);
            }

            return NoContent();
        }

        [HttpDelete("{pointOfInterestId}")]
        public async Task<IActionResult> DeletePointOfInterest(int cityId, int pointOfInterestId)
        {
            try
            {
                var poi = await _cityInfoRepositiory.GetPointOfinterestForCity(cityId, pointOfInterestId);
                if (poi == null)
                {
                    return NotFound();
                }
                await _cityInfoRepositiory.DeletePointOfInterestForCity(cityId, pointOfInterestId);

                //await _cityInfoRepositiory.DeletePointOfInterest(cityId, poi);
                _mailService.Send("Point of interest deleted", $"Point of interest {poi.Name} with id {pointOfInterestId} was deleted.");
            }
            catch (ArgumentException e)
            {
                return NotFound(e.Message);
            }

            return Ok();
        }
    }
}
