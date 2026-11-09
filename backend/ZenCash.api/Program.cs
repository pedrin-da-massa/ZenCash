
using Microsoft.EntityFrameworkCore;
using ZenCash.Api.Data;
using ZenCash.Api.Repositories;
using ZenCash.Api.Repositories.Interfaces;
using ZenCash.Api.Services;
using ZenCash.Api.Services.Interfaces;
using ZenCash.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Conexão com MySQL
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

// Injeção de dependência: Repositórios
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();

// Injeção de dependência: Serviços
builder.Services.AddScoped<IUsuarioService, UsuarioService>();

var app = builder.Build();

// Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Teste de conexão com o banco
app.MapGet("/db-status", async (ZenCashContext db) =>
{
    var conectado = await db.Database.CanConnectAsync();

    return conectado
        ? Results.Ok("ZenCash conectado ao MySQL!")
        : Results.Problem("Falha na conexão com o MySQL.");
});
app.MapUsuarioEndpoints();
app.Run();
