namespace ZenCash.Api.DTOs;

public class ContaDTO
{
    public int Id { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public DateTime Vencimento { get; set; }
    public bool Paga { get; set; }
}