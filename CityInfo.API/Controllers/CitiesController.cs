using CityInfo.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers;

[ApiController]
[Route("api/cities")]
public class CitiesController : ControllerBase
{
    private CitiesDataStore _dataStore;

    public CitiesController(CitiesDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public IActionResult GetCities()
    {
        return Ok(_dataStore.GetCities());
    }

    [HttpGet]
    [Route("{id}")]
    public ActionResult<CityDto> GetCity(int id)
    {
        var city = _dataStore.GetCity(id);
        if (city == null)
        {
            return NotFound();
        }
        return Ok(city);
    }
    
}
