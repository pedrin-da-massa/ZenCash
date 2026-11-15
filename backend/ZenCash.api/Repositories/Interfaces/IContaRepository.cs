
using ZenCash.Api.Models;

namespace ZenCash.Api.Repositories.Interfaces;

public interface IContaRepository
{
    Task<List<Conta>> ListarPorUsuarioAsync(int usuarioId);

    Task<Conta?> BuscarPorIdAsync(int id, int usuarioId);

    Task AdicionarAsync(Conta conta);

    Task AtualizarAsync(Conta conta);

    Task RemoverAsync(Conta conta);
}
