
using Microsoft.EntityFrameworkCore;
using ZenCash.Api.Data;
using ZenCash.Api.Models;
using ZenCash.Api.Repositories.Interfaces;

namespace ZenCash.Api.Repositories;

public class ContaRepository : IContaRepository
{
    private readonly ZenCashContext _context;

    public ContaRepository(ZenCashContext context)
    {
        _context = context;
    }

    public async Task<List<Conta>> ListarPorUsuarioAsync(int usuarioId)
    {
        return await _context.Contas
            .AsNoTracking()
            .Where(c => c.UsuarioId == usuarioId)
            .OrderBy(c => c.Vencimento)
            .ToListAsync();
    }

    public async Task<Conta?> BuscarPorIdAsync(int id, int usuarioId)
    {
        return await _context.Contas
            .FirstOrDefaultAsync(c => c.Id == id && c.UsuarioId == usuarioId);
    }

    public async Task AdicionarAsync(Conta conta)
    {
        await _context.Contas.AddAsync(conta);
        await _context.SaveChangesAsync();
    }

    public async Task AtualizarAsync(Conta conta)
    {
        _context.Contas.Update(conta);
        await _context.SaveChangesAsync();
    }

    public async Task RemoverAsync(Conta conta)
    {
        _context.Contas.Remove(conta);
        await _context.SaveChangesAsync();
    }
}
