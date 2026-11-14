
using ZenCash.Api.DTOs;

namespace ZenCash.Api.Services.Interfaces;

public interface ITransacaoService
{
    Task<List<TransacaoDTO>> ListarPorUsuarioAsync(int usuarioId);

    Task<TransacaoDTO?> BuscarPorIdAsync(int id, int usuarioId);

    Task<TransacaoDTO> AdicionarAsync(int usuarioId, TransacaoDTO transacao);

    Task<bool> AtualizarAsync(int id, int usuarioId, TransacaoDTO transacao);

    Task<bool> RemoverAsync(int id, int usuarioId);
}
