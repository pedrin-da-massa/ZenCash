
namespace ZenCash.Api.DTOs;

public class MetaDTO
{
    public int Id { get; set; }

    public string Nome { get; set; } = string.Empty;

    public decimal ValorObjetivo { get; set; }

    public decimal ValorAtual { get; set; }

    public DateTime? Prazo { get; set; }
}
