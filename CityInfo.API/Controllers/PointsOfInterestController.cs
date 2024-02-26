using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [Route("api/cities/{cityId}/pointsofinterest")]
    [ApiController]
    public class PointsOfInterestController : ControllerBase
    {
        private ILogger<PointsOfInterestController> _logger;
        private ILocalMailService _mailService;
        private CitiesDataStore _dataStore;

        public PointsOfInterestController(CitiesDataStore dataStore, ILogger<PointsOfInterestController> logger, ILocalMailService mailService)
        {
            _logger = logger;
            _mailService = mailService;
            _dataStore = dataStore;
        }

        [HttpGet]
        public IActionResult GetPointsOfInterest(int cityId)
        {
            var city = _dataStore.GetCity(cityId);
            if (city == null)
            {
                return NotFound();
            }
            
            return Ok(city.PointsOfInterests);
        }

        [HttpGet("{pointOfInterestId}", Name = "GetPointOfInterest")]
        public IActionResult GetPointOfInterest(int cityId, int pointOfInterestId)
        {
            var city = _dataStore.GetCity(cityId);
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
        public IActionResult AddPointOfInterest(int cityId, [FromBody] PointOfInterestForCreationDto pointOfInterest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var city = _dataStore.GetCity(cityId);

            if (city == null)
            {
                return NotFound();
            }

            var poi = _dataStore.CreatePointOfInterest(cityId, pointOfInterest);
            
            return CreatedAtRoute("GetPointOfInterest", new
            {
                cityId = cityId,
                pointOfInterestId = poi.Id
            }, poi);
        }


        [HttpPut("{pointOfInterestId}")]
        public IActionResult UpdatePointOfInterest(int cityId, int pointOfInterestId, [FromBody] PointOfInterestForUpdateDto update)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var city = _dataStore.GetCity(cityId);

            if (city == null)
            {
                return NotFound();
            }

            var poi = city.PointsOfInterests.SingleOrDefault(x => x.Id == pointOfInterestId);
            if (poi == null ) 
            {
                return NotFound();
            }

            poi.Name = update.Name;
            poi.Description = update.Description;

            return NoContent();
        }

        [HttpPatch("{pointOfInterestId}")]
        public IActionResult PartiallyUpdatePointOfInterest(int cityId, int pointOfInterestId, 
            JsonPatchDocument<PointOfInterestForUpdateDto> patchDocument)
        {
            var city = _dataStore.GetCity(cityId);

            if (city == null)
            {
                return NotFound();
            }

            var poi = city.PointsOfInterests.SingleOrDefault(x => x.Id == pointOfInterestId);
            if (poi == null)
            {
                return NotFound();
            }
            
            var update = new PointOfInterestForUpdateDto() {  Name = poi.Name, Description = poi.Description }; 
            patchDocument.ApplyTo(update, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!TryValidateModel(update))
            {
                return BadRequest(ModelState);
            }

            poi.Name = update.Name;
            poi.Description = update.Description;

            return NoContent();
        }

        [HttpDelete("{pointOfInterestId}")]
        public IActionResult DeletePointOfInterest(int cityId, int pointOfInterestId)
        {
            var city = _dataStore.GetCity(cityId);

            if (city == null)
            {
                return NotFound();
            }

            var poi = city.PointsOfInterests.SingleOrDefault(x => x.Id == pointOfInterestId);
            if (poi != null)
            {
                city.PointsOfInterests.Remove(poi);
            }

            _mailService.Send("Point of interest deleted", $"Point of interest {poi.Name} with id {poi.Id} was deleted.");
            return Ok();
        }

    }
}
