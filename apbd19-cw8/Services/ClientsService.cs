using apbd19_cw8.Models.DTOs;
using Microsoft.Data.SqlClient;

namespace apbd19_cw8.Services;

/// <summary>
/// Serwis zarządzający klientami
/// </summary>
public class ClientsService : IClientsService
{
    private readonly string? _connectionString;
    public ClientsService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }
    
    /// <summary>
    /// GET /api/clients/{id}/trips
    /// Metoda pozyskująca wszystkie wycieczki powiązane z konkretnym klientem.
    /// </summary>
    public async Task<List<TripDto>> GetClientTrips(int id)
    {
        var trips = new List<TripDto>();

        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            // Sprawdzenie, czy istnieje klient
            string query = @"SELECT 1 FROM Client WHERE IdClient = @id";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id", id); 
                var result = await command.ExecuteScalarAsync();
                if (result == null)
                {
                    connection.Close();
                    throw new Exception("Client does not exist");
                }
            }
            query = @"SELECT ct.IdTrip, t.Name, t.Description, t.DateFrom, t.DateTo, t.MaxPeople, ct.RegisteredAt, ct.PaymentDate FROM Client_Trip ct INNER JOIN Trip t ON t.IdTrip = ct.IdTrip WHERE ct.IdClient = @id";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id", id);
                using (var reader = await command.ExecuteReaderAsync())
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
                        });
                    }
                }
            }
        }
        return trips;
    }

    /// <summary>
    /// POST /api/clients
    /// Metoda tworząca nowy rekord klienta.
    /// </summary>
    public async Task<int> AddClient(ClientDto client)
    {
        string query = @"INSERT INTO Client (FirstName, LastName, Email, Telephone, Pesel) VALUES (@FirstName, @LastName, @Email, @Telephone, @Pesel); SELECT SCOPE_IDENTITY()";
        
        if (client == null) throw new ArgumentNullException(nameof(client));
        if (string.IsNullOrEmpty(client.FirstName) || 
            string.IsNullOrEmpty(client.LastName) || 
            string.IsNullOrEmpty(client.Email) || 
            string.IsNullOrEmpty(client.Telephone) || 
            string.IsNullOrEmpty(client.Pesel))
            throw new Exception("All data is required.");
        
        if (!client.Email.Contains("@")) throw new Exception("Email is invalid.");
        if (client.Pesel.Length != 11) throw new Exception("Pesel must be 11 characters long.");
        
        using (SqlConnection connection = new SqlConnection(_connectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@FirstName", client.FirstName);
            command.Parameters.AddWithValue("@LastName", client.LastName);
            command.Parameters.AddWithValue("@Email", client.Email);
            command.Parameters.AddWithValue("@Telephone", client.Telephone);
            command.Parameters.AddWithValue("@Pesel", client.Pesel);
            
            await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }
    }

    /// <summary>
    /// PUT /api/clients/{id}/trips/{tripId}
    /// Metoda rejestruje klienta na konkretną wycieczkę.
    /// </summary>
    public async Task<bool> RegisterClientForTrip(int clientId, int tripId)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            // Sprawdzenie, czy istnieje klient
            string query = @"SELECT 1 FROM Client WHERE IdClient = @id";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id", clientId); 
                var result = await command.ExecuteScalarAsync();
                if (result == null)
                {
                    connection.Close();
                    throw new Exception("Client does not exist");
                }
            }
            // Sprawdzenie, czy istnieje wycieczka 
            query = @"SELECT 1 FROM Trip WHERE IdTrip = @id";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id", tripId); 
                var result = await command.ExecuteScalarAsync();
                if (result == null)
                {
                    connection.Close();
                    throw new Exception("Trip does not exist");
                }
            }
            // Sprawdzenie, czy nie została osiągnięta maksymalna liczba uczestników
            query = @"SELECT t.MaxPeople, COUNT(ct.IdClient) FROM Client_Trip ct INNER JOIN Trip t ON t.IdTrip = ct.IdTrip GROUP BY t.IdTrip, t.MaxPeople HAVING t.IdTrip = @id";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id", tripId);
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        if (reader.GetInt32(0) <= reader.GetInt32(1))
                        {
                            reader.Close();
                            connection.Close();
                            throw new Exception("Max number of clients in the trip");
                        }
                    }
                }
            }
            // Sprawdzenie, czy istnieje wycieczka 
            query = @"SELECT 1 FROM Client_Trip WHERE IdClient = @clientId AND IdTrip = @tripId";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@clientId", clientId);
                command.Parameters.AddWithValue("@tripId", tripId); 
                var result = await command.ExecuteScalarAsync();
                if (result != null)
                {
                    connection.Close();
                    throw new Exception("Client already registered to this trip");
                }
            }
            // Właściwe wstawienie rekordu wycieczki 
            query = "INSERT INTO Client_Trip (IdClient, IdTrip, RegisteredAt) VALUES (@clientId, @tripId, convert(varchar(100), getdate(), 112))";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
               command.Parameters.AddWithValue("@clientId", clientId);
               command.Parameters.AddWithValue("@tripId", tripId);
               int rowsAffected = await command.ExecuteNonQueryAsync();
               
               connection.Close();
               return rowsAffected > 0;
            }
        }
    }
    
    /// <summary>
    /// DELETE /api/clients/{id}/trips/{tripId}
    /// Metoda usuwa klienta z wycieczki.
    /// </summary>
    public async Task<bool> UnregisterClientFromTrip(int clientId, int tripId)
    {
        string query = @"DELETE FROM Client_Trip WHERE IdClient = @clientId AND IdTrip = @tripId";
        
        using (SqlConnection connection = new SqlConnection(_connectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@clientId", clientId);
            command.Parameters.AddWithValue("@tripId", tripId);
            
            await connection.OpenAsync();

            int rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }
    }
}