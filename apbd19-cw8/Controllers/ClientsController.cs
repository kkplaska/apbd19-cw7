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

    [HttpGet]
    public async Task<IActionResult> GetClients()
    {
        var clients = new List<ClientDto>(){new ClientDto
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@gmail.com",
            Telephone = "67386478326",
            Pesel = "Pesel",
        }};
        return Ok(clients);
    }
    
    [HttpGet("{id}/trips")]
    public async Task<IActionResult> GetClientTrips(int id)
    {
        var clientTrips = await _clientsService.GetClientTrips(id);
        if (clientTrips.IsNullOrEmpty()) return NotFound();
        return Ok(clientTrips);
    }
    
    [HttpPost]
    public async Task<IActionResult> AddClient(ClientDto client)
    {
        int id = await _clientsService.AddClient(client);
        return Created("New client created", id);
    }

    [HttpPut("{id}/trips/{tripId}")]
    public async Task<IActionResult> RegisterClientForTrip(int id, int tripId)
    {
        bool result = await _clientsService.RegisterClientForTrip(id, tripId);
        if (!result) return BadRequest();
        return NoContent();
    }

    [HttpDelete("{id}/trips/{tripId}")]
    public async Task<IActionResult> UnregisterClientFromTrip(int id, int tripId)
    {
        throw new NotImplementedException();
    }
    
}