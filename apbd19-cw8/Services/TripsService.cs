using apbd19_cw8.Models.DTOs;
using Microsoft.Data.SqlClient;

namespace apbd19_cw8.Services;

public class TripsService : ITripsService
{
    private readonly string _connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=APBD;Integrated Security=True;";
    
    // DTO - Data Transfer Object
    public async Task<List<TripDTO>> GetTrips()
    {
        var trips = new List<TripDTO>();

        string sqlCommand = "SELECT IdTrip, Name FROM Trip";
        
        using (SqlConnection conn = new SqlConnection(_connectionString))
        using (SqlCommand cmd = new SqlCommand(sqlCommand, conn))
        {
            await conn.OpenAsync();

            using (var reader = await cmd.ExecuteReaderAsync()) // Zaczeka na pobranie kursora
            {
                while (await reader.ReadAsync())
                {
                    trips.Add(new TripDTO()
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                    });   
                }
            }
        }
        
        
        return trips;
    }
}