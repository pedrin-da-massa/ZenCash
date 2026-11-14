
using ZenCash.Api.Models;

namespace ZenCash.Api.Repositories.Interfaces;

public interface IMetaRepository
{
    Task<List<Meta>> ListarPorUsuarioAsync(int usuarioId);

    Task<Meta?> BuscarPorIdAsync(int id, int usuarioId);

    Task AdicionarAsync(Meta meta);

    Task AtualizarAsync(Meta meta);

    Task RemoverAsync(Meta meta);
}
