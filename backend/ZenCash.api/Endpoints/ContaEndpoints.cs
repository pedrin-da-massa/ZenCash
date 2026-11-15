
using System.Security.Claims;
using ZenCash.Api.DTOs;
using ZenCash.Api.Services.Interfaces;

namespace ZenCash.Api.Endpoints;

public static class ContaEndpoints
{
    public static void MapContaEndpoints(this IEndpointRouteBuilder app)
    {
        var grupo = app.MapGroup("/api/contas")
            .RequireAuthorization();

        grupo.MapGet("/", async (
            ClaimsPrincipal usuario,
            IContaService service) =>
        {
            var usuarioId = ObterUsuarioId(usuario);

            if (usuarioId == null)
                return Results.Unauthorized();

            var contas = await service.ListarPorUsuarioAsync(usuarioId.Value);

            return Results.Ok(contas);
        });

        grupo.MapGet("/{id:int}", async (
            int id,
            ClaimsPrincipal usuario,
            IContaService service) =>
        {
            var usuarioId = ObterUsuarioId(usuario);

            if (usuarioId == null)
                return Results.Unauthorized();

            var conta = await service.BuscarPorIdAsync(id, usuarioId.Value);

            return conta == null
                ? Results.NotFound()
                : Results.Ok(conta);
        });

        grupo.MapPost("/", async (
            ContaDTO conta,
            ClaimsPrincipal usuario,
            IContaService service) =>
        {
            var usuarioId = ObterUsuarioId(usuario);

            if (usuarioId == null)
                return Results.Unauthorized();

            try
            {
                var criada = await service.AdicionarAsync(usuarioId.Value, conta);

                return Results.Created($"/api/contas/{criada.Id}", criada);
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { mensagem = ex.Message });
            }
        });

        grupo.MapPut("/{id:int}", async (
            int id,
            ContaDTO conta,
            ClaimsPrincipal usuario,
            IContaService service) =>
        {
            var usuarioId = ObterUsuarioId(usuario);

            if (usuarioId == null)
                return Results.Unauthorized();

            try
            {
                var atualizada = await service.AtualizarAsync(id, usuarioId.Value, conta);

                return atualizada
                    ? Results.NoContent()
                    : Results.NotFound();
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { mensagem = ex.Message });
            }
        });

        grupo.MapDelete("/{id:int}", async (
            int id,
            ClaimsPrincipal usuario,
            IContaService service) =>
        {
            var usuarioId = ObterUsuarioId(usuario);

            if (usuarioId == null)
                return Results.Unauthorized();

            var removida = await service.RemoverAsync(id, usuarioId.Value);

            return removida
                ? Results.NoContent()
                : Results.NotFound();
        });
    }

    private static int? ObterUsuarioId(ClaimsPrincipal usuario)
    {
        var id = usuario.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? usuario.FindFirst("sub")?.Value;

        return int.TryParse(id, out var usuarioId)
            ? usuarioId
            : null;
    }
}
