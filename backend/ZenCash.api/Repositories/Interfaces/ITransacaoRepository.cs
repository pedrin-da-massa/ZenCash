
using ZenCash.Api.Models;

namespace ZenCash.Api.Repositories.Interfaces;

public interface ITransacaoRepository
{
    Task<List<Transacao>> ListarPorUsuarioAsync(int usuarioId);

    Task<Transacao?> BuscarPorIdAsync(int id, int usuarioId);

    Task AdicionarAsync(Transacao transacao);

    Task AtualizarAsync(Transacao transacao);

    Task RemoverAsync(Transacao transacao);
}
