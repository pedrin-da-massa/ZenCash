
using ZenCash.Api.DTOs;
using ZenCash.Api.Models;
using ZenCash.Api.Repositories.Interfaces;
using ZenCash.Api.Services.Interfaces;

namespace ZenCash.Api.Services;

public class MetaService : IMetaService
{
    private readonly IMetaRepository _metaRepository;

    public MetaService(IMetaRepository metaRepository)
    {
        _metaRepository = metaRepository;
    }

    public async Task<List<MetaDTO>> ListarPorUsuarioAsync(int usuarioId)
    {
        var metas = await _metaRepository.ListarPorUsuarioAsync(usuarioId);

        return metas.Select(m => new MetaDTO
        {
            Id = m.Id,
            Nome = m.Nome,
            ValorObjetivo = m.ValorObjetivo,
            ValorAtual = m.ValorAtual,
            Prazo = m.Prazo
        }).ToList();
    }

    public async Task<MetaDTO?> BuscarPorIdAsync(int id, int usuarioId)
    {
        var meta = await _metaRepository.BuscarPorIdAsync(id, usuarioId);

        if (meta == null)
            return null;

        return new MetaDTO
        {
            Id = meta.Id,
            Nome = meta.Nome,
            ValorObjetivo = meta.ValorObjetivo,
            ValorAtual = meta.ValorAtual,
            Prazo = meta.Prazo
        };
    }

    public async Task<MetaDTO> AdicionarAsync(int usuarioId, MetaDTO meta)
    {
        ValidarMeta(meta);

        var novaMeta = new Meta
        {
            UsuarioId = usuarioId,
            Nome = meta.Nome.Trim(),
            ValorObjetivo = meta.ValorObjetivo,
            ValorAtual = meta.ValorAtual,
            Prazo = meta.Prazo
        };

        await _metaRepository.AdicionarAsync(novaMeta);

        meta.Id = novaMeta.Id;

        return await BuscarPorIdAsync(novaMeta.Id, usuarioId)
            ?? throw new InvalidOperationException("Erro ao cadastrar meta.");
    }

    public async Task<bool> AtualizarAsync(int id, int usuarioId, MetaDTO meta)
    {
        ValidarMeta(meta);

        var metaExistente = await _metaRepository.BuscarPorIdAsync(id, usuarioId);

        if (metaExistente == null)
            return false;

        metaExistente.Nome = meta.Nome.Trim();
        metaExistente.ValorObjetivo = meta.ValorObjetivo;
        metaExistente.ValorAtual = meta.ValorAtual;
        metaExistente.Prazo = meta.Prazo;

        await _metaRepository.AtualizarAsync(metaExistente);

        return true;
    }

    public async Task<bool> RemoverAsync(int id, int usuarioId)
    {
        var meta = await _metaRepository.BuscarPorIdAsync(id, usuarioId);

        if (meta == null)
            return false;

        await _metaRepository.RemoverAsync(meta);

        return true;
    }

    private static void ValidarMeta(MetaDTO meta)
    {
        if (string.IsNullOrWhiteSpace(meta.Nome))
            throw new ArgumentException("O nome da meta é obrigatório.");

        if (meta.ValorObjetivo <= 0)
            throw new ArgumentException("O valor objetivo deve ser maior que zero.");

        if (meta.ValorAtual < 0)
            throw new ArgumentException("O valor atual não pode ser negativo.");

        if (meta.ValorAtual > meta.ValorObjetivo)
            throw new ArgumentException("O valor atual não pode ultrapassar o objetivo.");

if (meta.Prazo.HasValue && meta.Prazo.Value.Date < DateTime.Today)
    throw new ArgumentException("O prazo não pode estar no passado.");
    }
}
