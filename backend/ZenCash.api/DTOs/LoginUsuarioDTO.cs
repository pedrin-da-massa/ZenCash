namespace ZenCash.Api.DTOs;

public class LoginUsuarioDTO
{
    public string Token { get; set; } = string.Empty;

    public DateTime ExpiraEm { get; set; }

    public UsuarioDTO Usuario { get; set; } = new();
}