
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ZenCash.Api.Configuration;
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

// Configurações do JWT
var jwtSettings = builder.Configuration
    .GetSection("JwtSettings")
    .Get<JwtSettings>()
    ?? throw new InvalidOperationException(
        "Configurações do JWT não encontradas."
    );

if (string.IsNullOrWhiteSpace(jwtSettings.SecretKey))
{
    throw new InvalidOperationException(
        "A chave secreta do JWT não foi configurada."
    );
}

byte[] jwtKey;

try
{
    jwtKey = Convert.FromBase64String(jwtSettings.SecretKey);
}
catch (FormatException)
{
    throw new InvalidOperationException(
        "A chave secreta do JWT não está em um formato válido."
    );
}

if (jwtKey.Length < 32)
{
    throw new InvalidOperationException(
        "A chave secreta do JWT precisa ter pelo menos 32 bytes."
    );
}

if (string.IsNullOrWhiteSpace(jwtSettings.Issuer) ||
    string.IsNullOrWhiteSpace(jwtSettings.Audience) ||
    jwtSettings.ExpirationMinutes <= 0)
{
    throw new InvalidOperationException(
        "As configurações do JWT estão incompletas."
    );
}

// Autenticação JWT
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtSettings.Issuer,

                ValidateAudience = true,
                ValidAudience = jwtSettings.Audience,

                ValidateLifetime = true,

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(jwtKey),

                ClockSkew = TimeSpan.FromSeconds(30)
            };
    });

// Autorização
builder.Services.AddAuthorization();

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

// Autenticação e autorização
app.UseAuthentication();
app.UseAuthorization();

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
