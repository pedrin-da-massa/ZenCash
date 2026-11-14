
using System.Security.Claims;
using ZenCash.Api.DTOs;
using ZenCash.Api.Services.Interfaces;

namespace ZenCash.Api.Endpoints;

public static class TransacaoEndpoints
{
    public static void MapTransacaoEndpoints(this IEndpointRouteBuilder app)
    {
        var grupo = app.MapGroup("/api/transacoes")
            .RequireAuthorization();

        grupo.MapGet("/", async (
            ClaimsPrincipal usuario,
            ITransacaoService service) =>
        {
            var usuarioId = ObterUsuarioId(usuario);

            if (usuarioId == null)
                return Results.Unauthorized();

            var transacoes = await service.ListarPorUsuarioAsync(usuarioId.Value);

            return Results.Ok(transacoes);
        });

        grupo.MapGet("/{id:int}", async (
            int id,
            ClaimsPrincipal usuario,
            ITransacaoService service) =>
        {
            var usuarioId = ObterUsuarioId(usuario);

            if (usuarioId == null)
                return Results.Unauthorized();

            var transacao = await service.BuscarPorIdAsync(id, usuarioId.Value);

            return transacao == null
                ? Results.NotFound()
                : Results.Ok(transacao);
        });

        grupo.MapPost("/", async (
            TransacaoDTO transacao,
            ClaimsPrincipal usuario,
            ITransacaoService service) =>
        {
            var usuarioId = ObterUsuarioId(usuario);

            if (usuarioId == null)
                return Results.Unauthorized();

            try
            {
                var criada = await service.AdicionarAsync(usuarioId.Value, transacao);
                return Results.Created($"/api/transacoes/{criada.Id}", criada);
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { mensagem = ex.Message });
            }
        });

        grupo.MapPut("/{id:int}", async (
            int id,
            TransacaoDTO transacao,
            ClaimsPrincipal usuario,
            ITransacaoService service) =>
        {
            var usuarioId = ObterUsuarioId(usuario);

            if (usuarioId == null)
                return Results.Unauthorized();

            try
            {
                var atualizada = await service.AtualizarAsync(id, usuarioId.Value, transacao);

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
            ITransacaoService service) =>
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
