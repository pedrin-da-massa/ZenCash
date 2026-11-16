namespace ZenCash.Api.DTOs;

public class LoginUsuarioRequest
{
    public string Email { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
}