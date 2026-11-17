
using ZenCash.Api.DTOs;
using ZenCash.Api.Models;
using ZenCash.Api.Repositories.Interfaces;
using ZenCash.Api.Services.Interfaces;

namespace ZenCash.Api.Services;

public class ContaService : IContaService
{
    private readonly IContaRepository _repository;

    public ContaService(IContaRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<ContaDTO>> ListarPorUsuarioAsync(int usuarioId)
    {
        var contas = await _repository.ListarPorUsuarioAsync(usuarioId);

        return contas.Select(c => new ContaDTO
        {
            Id = c.Id,
            Descricao = c.Descricao,
            Valor = c.Valor,
            Vencimento = c.Vencimento,
            Paga = c.Paga
        }).ToList();
    }

    public async Task<ContaDTO?> BuscarPorIdAsync(int id, int usuarioId)
    {
        var conta = await _repository.BuscarPorIdAsync(id, usuarioId);

        if (conta == null)
            return null;

        return new ContaDTO
        {
            Id = conta.Id,
            Descricao = conta.Descricao,
            Valor = conta.Valor,
            Vencimento = conta.Vencimento,
            Paga = conta.Paga
        };
    }

    public async Task<ContaDTO> AdicionarAsync(int usuarioId, ContaDTO conta)
    {
        ValidarConta(conta);

        var novaConta = new Conta
        {
            UsuarioId = usuarioId,
            Descricao = conta.Descricao.Trim(),
            Valor = conta.Valor,
            Vencimento = conta.Vencimento,
            Paga = conta.Paga
        };

        await _repository.AdicionarAsync(novaConta);

        return new ContaDTO
        {
            Id = novaConta.Id,
            Descricao = novaConta.Descricao,
            Valor = novaConta.Valor,
            Vencimento = novaConta.Vencimento,
            Paga = novaConta.Paga
        };
    }

    public async Task<bool> AtualizarAsync(int id, int usuarioId, ContaDTO conta)
    {
        ValidarConta(conta);

        var existente = await _repository.BuscarPorIdAsync(id, usuarioId);

        if (existente == null)
            return false;

        existente.Descricao = conta.Descricao.Trim();
        existente.Valor = conta.Valor;
        existente.Vencimento = conta.Vencimento;
        existente.Paga = conta.Paga;

        await _repository.AtualizarAsync(existente);

        return true;
    }

    public async Task<bool> RemoverAsync(int id, int usuarioId)
    {
        var conta = await _repository.BuscarPorIdAsync(id, usuarioId);

        if (conta == null)
            return false;

        await _repository.RemoverAsync(conta);

        return true;
    }

    private static void ValidarConta(ContaDTO conta)
    {
        if (string.IsNullOrWhiteSpace(conta.Descricao))
            throw new ArgumentException("A descrição é obrigatória.");

        if (conta.Valor <= 0)
            throw new ArgumentException("O valor deve ser maior que zero.");
    }
}
