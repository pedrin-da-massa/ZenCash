
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ZenCash.Api.Configuration;
using ZenCash.Api.DTOs;
using ZenCash.Api.Models;
using ZenCash.Api.Repositories.Interfaces;
using ZenCash.Api.Services.Interfaces;

namespace ZenCash.Api.Services;

public class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository _repository;
    private readonly PasswordHasher<Usuario> _passwordHasher;
    private readonly JwtSettings _jwtSettings;

    public UsuarioService(
        IUsuarioRepository repository,
        IOptions<JwtSettings> jwtOptions)
    {
        _repository = repository;
        _passwordHasher = new PasswordHasher<Usuario>();
        _jwtSettings = jwtOptions.Value;
    }

    public async Task<UsuarioDTO> CadastrarAsync(
        string nome,
        string email,
        string senha)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("O nome é obrigatório.");

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("O e-mail é obrigatório.");

        if (string.IsNullOrWhiteSpace(senha) || senha.Length < 8)
            throw new ArgumentException(
                "A senha deve ter pelo menos 8 caracteres."
            );

        email = email.Trim().ToLowerInvariant();

        if (await _repository.EmailExisteAsync(email))
            throw new InvalidOperationException(
                "Este e-mail já está cadastrado."
            );

        var usuario = new Usuario
        {
            Nome = nome.Trim(),
            Email = email,
            DataCadastro = DateTime.Now
        };

        usuario.SenhaHash = _passwordHasher.HashPassword(
            usuario,
            senha
        );

        await _repository.AdicionarAsync(usuario);

        return new UsuarioDTO
        {
            Id = usuario.Id,
            Nome = usuario.Nome,
            Email = usuario.Email
        };
    }

    public async Task<UsuarioDTO?> BuscarPorIdAsync(int id)
    {
        var usuario = await _repository.BuscarPorIdAsync(id);

        if (usuario == null)
            return null;

        return new UsuarioDTO
        {
            Id = usuario.Id,
            Nome = usuario.Nome,
            Email = usuario.Email
        };
    }

    public async Task<LoginUsuarioDTO?> LoginAsync(
        string email,
        string senha)
    {
        if (string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(senha))
        {
            return null;
        }

        email = email.Trim().ToLowerInvariant();

        var usuario = await _repository.BuscarPorEmailAsync(email);

        if (usuario == null)
            return null;

        var resultado = _passwordHasher.VerifyHashedPassword(
            usuario,
            usuario.SenhaHash,
            senha
        );

        if (resultado == PasswordVerificationResult.Failed)
            return null;

        var expiracao = DateTime.UtcNow.AddMinutes(
            _jwtSettings.ExpirationMinutes
        );

        var claims = new[]
        {
            new Claim(
                ClaimTypes.NameIdentifier,
                usuario.Id.ToString()
            ),
            new Claim(ClaimTypes.Name, usuario.Nome),
            new Claim(ClaimTypes.Email, usuario.Email)
        };

        var chave = new SymmetricSecurityKey(
            Convert.FromBase64String(_jwtSettings.SecretKey)
        );

        var credenciais = new SigningCredentials(
            chave,
            SecurityAlgorithms.HmacSha256
        );

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: expiracao,
            signingCredentials: credenciais
        );

        return new LoginUsuarioDTO
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            ExpiraEm = expiracao,
            Usuario = new UsuarioDTO
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email
            }
        };
    }
}
