
using ZenCash.Api.Services.Interfaces;

namespace ZenCash.Api.Endpoints;

public static class UsuarioEndpoints
{
    public static void MapUsuarioEndpoints(this WebApplication app)
    {
        var grupo = app.MapGroup("/api/usuarios")
            .WithTags("Usuários");

        // Cadastrar usuário
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
                return Results.BadRequest(new { mensagem = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Results.Conflict(new { mensagem = ex.Message });
            }
        });

        // Buscar usuário por ID
        grupo.MapGet("/{id:int}", async (
            int id,
            IUsuarioService service) =>
        {
            var usuario = await service.BuscarPorIdAsync(id);

            return usuario == null
                ? Results.NotFound(new { mensagem = "Usuário não encontrado." })
                : Results.Ok(usuario);
        });
    }
}

public record CadastroUsuarioRequest(
    string Nome,
    string Email,
    string Senha
);
