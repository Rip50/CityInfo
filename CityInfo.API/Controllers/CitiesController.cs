using CityInfo.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers;

[ApiController]
[Route("api/cities")]
[Authorize]
public class CitiesController : ControllerBase
{
    private ICityInfoRepository _dataStore;

    public CitiesController(ICityInfoRepository dataStore)
    {
        _dataStore = dataStore;
    }

    public async Task<IActionResult> GetCities()
    {
        return Ok(await _dataStore.GetCities());
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<ActionResult<CityDto>> GetCity(int id, bool includePointsOfInterest = false)
    {
        var city = await _dataStore.GetCity(id, includePointsOfInterest);
        if (city == null)
        {
            return NotFound();
        }
        return Ok(city);
    }
    
}
