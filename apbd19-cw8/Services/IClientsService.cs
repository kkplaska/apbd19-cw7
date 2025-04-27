using apbd19_cw8.Models.DTOs;

namespace apbd19_cw8.Services;

public interface IClientsService
{
    Task<List<TripDto>> GetClientTrips(int id);
    Task<int> AddClient(ClientDto client);
    Task<bool> RegisterClientForTrip(int clientId, int tripId);
    Task UnregisterClientFromTrip(int clientId, int tripId);
}