using intelligent_hostel;
using Npgsql;

var builder = WebApplication.CreateBuilder(
    new WebApplicationOptions { WebRootPath = "build"});
var app = builder.Build();
await Database.init();

var connectionString = "Host=localhost;Username=postgres;Password=postgres;Database=dormitory_db";
await using var dataSource = NpgsqlDataSource.Create(connectionString);
app.MapGet("/", async (HttpContext context) =>
{
    await using (var cmd = dataSource.CreateCommand("SELECT * FROM users"))
    await using (var reader = await cmd.ExecuteReaderAsync())
    {
        while (await reader.ReadAsync())
        {
            await context.Response.WriteAsync(reader.GetString(0));
        }
    }

});
app.Run();