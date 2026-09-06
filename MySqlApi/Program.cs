using Microsoft.EntityFrameworkCore;
using MySqlApi.DataBase;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(options => 
{
    options.ListenAnyIP(5000);
});
// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();

var connectionString = Environment.GetEnvironmentVariable("MySQLConnectionString");

if(string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Falha ao obter a cadeia de conexão: string nula");
}

builder.Services.AddDbContext<KobaContext>(option => 
    option.UseMySQL(connectionString));

var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Services.GetService<KobaContext>()!.Database.EnsureCreated();

app.Run();
