
using System.Security.Claims;
using ZenCash.Api.DTOs;
using ZenCash.Api.Services.Interfaces;

namespace ZenCash.Api.Endpoints;

public static class UsuarioEndpoints
{
    public static void MapUsuarioEndpoints(this WebApplication app)
    {
        var grupo = app.MapGroup("/api/usuarios")
            .WithTags("Usuários");

        // Cadastro: acesso público
        grupo.MapPost("/", async (
            CadastroUsuarioRequest request,
            IUsuarioService service) =>
        {
            try
            {
                var usuario = await service.CadastrarAsync(
                    request.Nome,
                    request.Email,
                    request.Senha
                );

                return Results.Created(
                    $"/api/usuarios/{usuario.Id}",
                    usuario
                );
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(
                    new { mensagem = ex.Message }
                );
            }
            catch (InvalidOperationException ex)
            {
                return Results.Conflict(
                    new { mensagem = ex.Message }
                );
            }
        });

        // Login: acesso público
        grupo.MapPost("/login", async (
            LoginUsuarioRequest request,
            IUsuarioService service) =>
        {
            var resultado = await service.LoginAsync(
                request.Email,
                request.Senha
            );

            return resultado == null
                ? Results.Unauthorized()
                : Results.Ok(resultado);
        });

        // Perfil: somente usuário autenticado
        grupo.MapGet("/me", async (
            ClaimsPrincipal usuarioLogado,
            IUsuarioService service) =>
        {
            var idClaim = usuarioLogado.FindFirst(
                ClaimTypes.NameIdentifier
            )?.Value;

            if (!int.TryParse(idClaim, out var id))
                return Results.Unauthorized();

            var usuario = await service.BuscarPorIdAsync(id);

            return usuario == null
                ? Results.NotFound(
                    new { mensagem = "Usuário não encontrado." }
                )
                : Results.Ok(usuario);
        })
        .RequireAuthorization();
    }
}

public record CadastroUsuarioRequest(
    string Nome,
    string Email,
    string Senha
);
