
using ZenCash.Api.DTOs;
using ZenCash.Api.Models;
using ZenCash.Api.Repositories.Interfaces;
using ZenCash.Api.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace ZenCash.Api.Services;

public class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository _repository;
    private readonly PasswordHasher<Usuario> _passwordHasher;

    public UsuarioService(IUsuarioRepository repository)
    {
        _repository = repository;
        _passwordHasher = new PasswordHasher<Usuario>();
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
}
