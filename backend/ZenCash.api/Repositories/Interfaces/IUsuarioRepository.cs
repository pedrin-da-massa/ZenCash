
using ZenCash.Api.Models;

namespace ZenCash.Api.Repositories.Interfaces;

public interface IUsuarioRepository
{
    Task<Usuario?> BuscarPorIdAsync(int id);

    Task<Usuario?> BuscarPorEmailAsync(string email);

    Task<bool> EmailExisteAsync(string email);

    Task AdicionarAsync(Usuario usuario);
}
