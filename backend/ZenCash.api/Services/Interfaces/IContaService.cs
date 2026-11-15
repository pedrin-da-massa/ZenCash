
using ZenCash.Api.DTOs;

namespace ZenCash.Api.Services.Interfaces;

public interface IContaService
{
    Task<List<ContaDTO>> ListarPorUsuarioAsync(int usuarioId);

    Task<ContaDTO?> BuscarPorIdAsync(int id, int usuarioId);

    Task<ContaDTO> AdicionarAsync(int usuarioId, ContaDTO conta);

    Task<bool> AtualizarAsync(int id, int usuarioId, ContaDTO conta);

    Task<bool> RemoverAsync(int id, int usuarioId);
}
