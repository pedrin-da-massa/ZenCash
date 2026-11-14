
using ZenCash.Api.DTOs;

namespace ZenCash.Api.Services.Interfaces;

public interface IMetaService
{
    Task<List<MetaDTO>> ListarPorUsuarioAsync(int usuarioId);

    Task<MetaDTO?> BuscarPorIdAsync(int id, int usuarioId);

    Task<MetaDTO> AdicionarAsync(int usuarioId, MetaDTO meta);

    Task<bool> AtualizarAsync(int id, int usuarioId, MetaDTO meta);

    Task<bool> RemoverAsync(int id, int usuarioId);
}
