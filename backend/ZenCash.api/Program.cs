
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
using Microsoft.OpenApi.Models;



var builder = WebApplication.CreateBuilder(args);

// Swagger
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Informe o token JWT recebido no login."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

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

builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("JwtSettings")
);


// Autenticação JWT
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
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

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                Console.WriteLine(
                    context.Request.Headers.ContainsKey("Authorization")
                    ? "JWT: Cabeçalho recebido."
                    : "JWT: Cabeçalho NÃO recebido."
                );

                return Task.CompletedTask;
            },

            OnAuthenticationFailed = context =>
            {
                Console.WriteLine(
                    $"JWT INVÁLIDO: {context.Exception.Message}"
                );

                return Task.CompletedTask;
            },

            OnTokenValidated = context =>
            {
                Console.WriteLine("JWT VALIDADO COM SUCESSO!");

                return Task.CompletedTask;
            }
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

// Injeção de dependência (DI): Repositórios
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<ITransacaoRepository, TransacaoRepository>();
builder.Services.AddScoped<IMetaRepository, MetaRepository>();

// Injeção de dependência (DI): Serviços
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<ITransacaoService, TransacaoService>();
builder.Services.AddScoped<IMetaService, MetaService>();

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
app.MapTransacaoEndpoints();
app.MapMetaEndpoints();

app.Run();
