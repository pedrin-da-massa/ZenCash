
using Microsoft.EntityFrameworkCore;
using ZenCash.Api.Data;
using ZenCash.Api.Models;
using ZenCash.Api.Repositories.Interfaces;

namespace ZenCash.Api.Repositories;

public class TransacaoRepository : ITransacaoRepository
{
    private readonly ZenCashContext _context;

    public TransacaoRepository(ZenCashContext context)
    {
        _context = context;
    }

    public async Task<List<Transacao>> ListarPorUsuarioAsync(int usuarioId)
    {
        return await _context.Transacoes
            .AsNoTracking()
            .Where(t => t.UsuarioId == usuarioId)
            .OrderByDescending(t => t.Data)
            .ToListAsync();
    }

    public async Task<Transacao?> BuscarPorIdAsync(int id, int usuarioId)
    {
        return await _context.Transacoes
            .FirstOrDefaultAsync(t => t.Id == id && t.UsuarioId == usuarioId);
    }

    public async Task AdicionarAsync(Transacao transacao)
    {
        await _context.Transacoes.AddAsync(transacao);
        await _context.SaveChangesAsync();
    }

    public async Task AtualizarAsync(Transacao transacao)
    {
        _context.Transacoes.Update(transacao);
        await _context.SaveChangesAsync();
    }

    public async Task RemoverAsync(Transacao transacao)
    {
        _context.Transacoes.Remove(transacao);
        await _context.SaveChangesAsync();
    }
}
