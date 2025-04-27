using apbd19_cw8.Services;

namespace apbd19_cw8;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllers();
        builder.Services.AddScoped<ITripsService, TripsService>();
        builder.Services.AddScoped<IClientsService, ClientsService>();

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment()) app.MapOpenApi();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}