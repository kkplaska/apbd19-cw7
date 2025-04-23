using apbd19_cw8.Services;
using Microsoft.AspNetCore.Mvc;

namespace apbd19_cw8.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TripsController : ControllerBase

// !!! Dependency injections
// http://localhost:5047/api/trips/
// POST też ma być DTO 
{
    private readonly ITripsService _tripsService;

    public TripsController(ITripsService tripsService)
    {
        _tripsService = tripsService;
    }

    [HttpGet]
    public async Task<IActionResult> GetTrips()
    {
        var trips = await _tripsService.GetTrips();
        return Ok(trips);
    }
}