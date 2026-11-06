using Microsoft.EntityFrameworkCore;
using ZenCash.Api.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Conexão com o MySQL
var connectionString = builder.Configuration
    .GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "String de conexão não encontrada."
    );

builder.Services.AddDbContext<ZenCashContext>(options =>
    options.UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString)
    )
);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Rota para testar o banco
app.MapGet("/db-status", async (ZenCashContext db) =>
{
    var conectado = await db.Database.CanConnectAsync();

    return conectado
        ? Results.Ok("ZenCash conectado ao MySQL!")
        : Results.Problem("Falha na conexão com o MySQL.");
});

app.Run();