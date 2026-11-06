
using ZenCash.Api.DTOs;

namespace ZenCash.Api.Services.Interfaces;

public interface IUsuarioService
{
    Task<UsuarioDTO> CadastrarAsync(
        string nome,
        string email,
        string senha
    );

    Task<UsuarioDTO?> BuscarPorIdAsync(int id);
}
