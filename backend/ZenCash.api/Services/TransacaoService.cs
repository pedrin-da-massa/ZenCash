
using ZenCash.Api.DTOs;
using ZenCash.Api.Models;
using ZenCash.Api.Repositories.Interfaces;
using ZenCash.Api.Services.Interfaces;

namespace ZenCash.Api.Services;

public class TransacaoService : ITransacaoService
{
    private readonly ITransacaoRepository _repository;

    public TransacaoService(ITransacaoRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<TransacaoDTO>> ListarPorUsuarioAsync(int usuarioId)
    {
        var transacoes = await _repository.ListarPorUsuarioAsync(usuarioId);

        return transacoes.Select(t => new TransacaoDTO
        {
            Id = t.Id,
            Descricao = t.Descricao,
            Valor = t.Valor,
            Tipo = t.Tipo,
            Data = t.Data
        }).ToList();
    }

    public async Task<TransacaoDTO?> BuscarPorIdAsync(int id, int usuarioId)
    {
        var transacao = await _repository.BuscarPorIdAsync(id, usuarioId);

        if (transacao == null)
            return null;

        return new TransacaoDTO
        {
            Id = transacao.Id,
            Descricao = transacao.Descricao,
            Valor = transacao.Valor,
            Tipo = transacao.Tipo,
            Data = transacao.Data
        };
    }

    public async Task<TransacaoDTO> AdicionarAsync(int usuarioId, TransacaoDTO transacao)
    {
        ValidarTransacao(transacao);

        var novaTransacao = new Transacao
        {
            UsuarioId = usuarioId,
            Descricao = transacao.Descricao.Trim(),
            Valor = transacao.Valor,
            Tipo = transacao.Tipo.Trim(),
            Data = transacao.Data
        };

        await _repository.AdicionarAsync(novaTransacao);

        return new TransacaoDTO
        {
            Id = novaTransacao.Id,
            Descricao = novaTransacao.Descricao,
            Valor = novaTransacao.Valor,
            Tipo = novaTransacao.Tipo,
            Data = novaTransacao.Data
        };
    }

    public async Task<bool> AtualizarAsync(int id, int usuarioId, TransacaoDTO transacao)
    {
        ValidarTransacao(transacao);

        var existente = await _repository.BuscarPorIdAsync(id, usuarioId);

        if (existente == null)
            return false;

        existente.Descricao = transacao.Descricao.Trim();
        existente.Valor = transacao.Valor;
        existente.Tipo = transacao.Tipo.Trim();
        existente.Data = transacao.Data;

        await _repository.AtualizarAsync(existente);

        return true;
    }

    public async Task<bool> RemoverAsync(int id, int usuarioId)
    {
        var transacao = await _repository.BuscarPorIdAsync(id, usuarioId);

        if (transacao == null)
            return false;

        await _repository.RemoverAsync(transacao);

        return true;
    }

    private static void ValidarTransacao(TransacaoDTO transacao)
    {
        if (string.IsNullOrWhiteSpace(transacao.Descricao))
            throw new ArgumentException("A descrição é obrigatória.");

        if (transacao.Valor <= 0)
            throw new ArgumentException("O valor deve ser maior que zero.");

        if (string.IsNullOrWhiteSpace(transacao.Tipo) ||
            (!transacao.Tipo.Trim().Equals("Receita", StringComparison.OrdinalIgnoreCase) &&
             !transacao.Tipo.Trim().Equals("Despesa", StringComparison.OrdinalIgnoreCase)))
        {
            throw new ArgumentException("O tipo deve ser Receita ou Despesa.");
        }
    }
}
