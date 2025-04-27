using apbd19_cw8.Models.DTOs;

namespace apbd19_cw8.Services;

public interface ITripsService
{
    Task<List<TripDto>> GetAllTrips();
}