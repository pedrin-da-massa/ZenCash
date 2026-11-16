
using System.Security.Claims;
using ZenCash.Api.DTOs;
using ZenCash.Api.Services.Interfaces;

namespace ZenCash.Api.Endpoints;

public static class MetaEndpoints
{
    public static void MapMetaEndpoints(this IEndpointRouteBuilder app)
    {
        var grupo = app.MapGroup("/api/metas")
            .RequireAuthorization();

        grupo.MapGet("/", async (ClaimsPrincipal usuario, IMetaService service) =>
        {
            var usuarioId = ObterUsuarioId(usuario);

            var metas = await service.ListarPorUsuarioAsync(usuarioId);

            return Results.Ok(metas);
        });

        grupo.MapGet("/{id:int}", async (int id, ClaimsPrincipal usuario, IMetaService service) =>
        {
            var usuarioId = ObterUsuarioId(usuario);

            var meta = await service.BuscarPorIdAsync(id, usuarioId);

            return meta == null
                ? Results.NotFound()
                : Results.Ok(meta);
        });

        grupo.MapPost("/", async (MetaDTO meta, ClaimsPrincipal usuario, IMetaService service) =>
        {
            try
            {
                var usuarioId = ObterUsuarioId(usuario);

                var novaMeta = await service.AdicionarAsync(usuarioId, meta);

                return Results.Created($"/api/metas/{novaMeta.Id}", novaMeta);
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { mensagem = ex.Message });
            }
        });

        grupo.MapPut("/{id:int}", async (int id, MetaDTO meta, ClaimsPrincipal usuario, IMetaService service) =>
        {
            try
            {
                var usuarioId = ObterUsuarioId(usuario);

                var atualizado = await service.AtualizarAsync(id, usuarioId, meta);

                return atualizado
                    ? Results.NoContent()
                    : Results.NotFound();
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { mensagem = ex.Message });
            }
        });

        grupo.MapDelete("/{id:int}", async (int id, ClaimsPrincipal usuario, IMetaService service) =>
        {
            var usuarioId = ObterUsuarioId(usuario);

            var removido = await service.RemoverAsync(id, usuarioId);

            return removido
                ? Results.NoContent()
                : Results.NotFound();
        });
    }

    private static int ObterUsuarioId(ClaimsPrincipal usuario)
    {
        var id = usuario.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? usuario.FindFirst("sub")?.Value;

        if (!int.TryParse(id, out var usuarioId))
            throw new UnauthorizedAccessException("Usuário inválido.");

        return usuarioId;
    }
}
