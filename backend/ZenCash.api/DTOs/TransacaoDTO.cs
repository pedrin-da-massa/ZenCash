
namespace ZenCash.Api.DTOs;

public class TransacaoDTO
{
    public int Id { get; set; }

    public string Descricao { get; set; } = string.Empty;

    public decimal Valor { get; set; }

    public string Tipo { get; set; } = string.Empty;

    public DateTime Data { get; set; }
}
