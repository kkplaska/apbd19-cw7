using apbd19_cw8.Models.DTOs;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;

namespace apbd19_cw8.Services;

public class TripsService : ITripsService
{
    private readonly string? _connectionString;
    public TripsService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }


    public async Task<List<TripDto>> GetAllTrips()
    {
        var trips = new List<TripDto>();

        var command =
            "SELECT t.IdTrip, t.Name, t.Description, t.DateFrom, t.DateTo, t.MaxPeople, STRING_AGG(c.Name, ', ') as Countries  FROM Trip t  INNER JOIN Country_Trip ct  INNER JOIN Country c  ON c.IdCountry = ct.IdCountry  ON t.IdTrip = ct.Idtrip  GROUP BY t.IdTrip, t.Name, t.Description, t.DateFrom, t.DateTo, t.MaxPeople ORDER BY t.IdTrip";

        using (var conn = new SqlConnection(_connectionString))
        using (var cmd = new SqlCommand(command, conn))
        {
            await conn.OpenAsync();

            using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    var idOrdinal = reader.GetOrdinal("IdTrip");
                    trips.Add(new TripDto
                    {
                        Id = reader.GetInt32(idOrdinal),
                        Name = reader.GetString(1),
                        Description = reader.GetString(2),
                        DateFrom = reader.GetDateTime(3),
                        DateTo = reader.GetDateTime(4),
                        MaxPeople = reader.GetInt32(5),
                        Countries = reader.GetString(6).Split(',').ToList().Select(x => new CountryDto{Name = x}).ToList()
                    });
                }
            }
        }

        return trips;
    }
}