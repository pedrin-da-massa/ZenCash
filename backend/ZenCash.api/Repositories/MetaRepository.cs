
using Microsoft.EntityFrameworkCore;
using ZenCash.Api.Data;
using ZenCash.Api.Models;
using ZenCash.Api.Repositories.Interfaces;

namespace ZenCash.Api.Repositories;

public class MetaRepository : IMetaRepository
{
    private readonly ZenCashContext _context;

    public MetaRepository(ZenCashContext context)
    {
        _context = context;
    }

    public async Task<List<Meta>> ListarPorUsuarioAsync(int usuarioId)
    {
        return await _context.Metas
            .AsNoTracking()
            .Where(m => m.UsuarioId == usuarioId)
            .OrderBy(m => m.Prazo)
            .ToListAsync();
    }

    public async Task<Meta?> BuscarPorIdAsync(int id, int usuarioId)
    {
        return await _context.Metas
            .FirstOrDefaultAsync(m => m.Id == id && m.UsuarioId == usuarioId);
    }

    public async Task AdicionarAsync(Meta meta)
    {
        await _context.Metas.AddAsync(meta);
        await _context.SaveChangesAsync();
    }

    public async Task AtualizarAsync(Meta meta)
    {
        _context.Metas.Update(meta);
        await _context.SaveChangesAsync();
    }

    public async Task RemoverAsync(Meta meta)
    {
        _context.Metas.Remove(meta);
        await _context.SaveChangesAsync();
    }
}
