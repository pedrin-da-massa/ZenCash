
namespace ZenCash.Api.Models;

public class Meta
{
    public int Id { get; set; }

    public int UsuarioId { get; set; }

    public string Nome { get; set; } = string.Empty;

    public decimal ValorObjetivo { get; set; }

    public decimal ValorAtual { get; set; }

    public DateTime? Prazo { get; set; }
}
