using apbd19_cw8.Models.DTOs;
using apbd19_cw8.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace apbd19_cw8.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ClientsController : ControllerBase
{
    private readonly IClientsService _clientsService;

    public ClientsController(IClientsService clientsService)
    {
        _clientsService = clientsService;
    }
    
    [HttpGet("{id}/trips")]
    public async Task<IActionResult> GetClientTrips(int id)
    {
        try
        {
            var clientTrips = await _clientsService.GetClientTrips(id);
            return Ok(clientTrips);
        }
        catch (Exception e)
        {
            return NotFound(e.Message);
        }
    }
    
    [HttpPost]
    public async Task<IActionResult> AddClient(ClientDto client)
    {
        int id;
        try
        {
            id = await _clientsService.AddClient(client);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
        return Created("New client created", id);
    }

    [HttpPut("{id}/trips/{tripId}")]
    public async Task<IActionResult> RegisterClientForTrip(int id, int tripId)
    {
        try
        {
            bool result = await _clientsService.RegisterClientForTrip(id, tripId);
            if(result) return BadRequest();
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}/trips/{tripId}")]
    public async Task<IActionResult> UnregisterClientFromTrip(int id, int tripId)
    {
        bool result = await _clientsService.UnregisterClientFromTrip(id, tripId);
        if (!result) return BadRequest();
        return NoContent();
    }
    
}