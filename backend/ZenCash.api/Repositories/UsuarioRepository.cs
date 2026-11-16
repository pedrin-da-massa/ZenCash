
using Microsoft.EntityFrameworkCore;
using ZenCash.Api.Data;
using ZenCash.Api.Models;
using ZenCash.Api.Repositories.Interfaces;

namespace ZenCash.Api.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly ZenCashContext _context;

    public UsuarioRepository(ZenCashContext context)
    {
        _context = context;
    }

    public async Task<Usuario?> BuscarPorIdAsync(int id)
    {
        return await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<Usuario?> BuscarPorEmailAsync(string email)
    {
        return await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<bool> EmailExisteAsync(string email)
    {
        return await _context.Usuarios
            .AnyAsync(u => u.Email == email);
    }

    public async Task AdicionarAsync(Usuario usuario)
    {
        await _context.Usuarios.AddAsync(usuario);

        await _context.SaveChangesAsync();
    }
}
