
namespace ZenCash.Api.Models;

public class Conta
{
    public int Id { get; set; }

    public int UsuarioId { get; set; }

    public string Descricao { get; set; } = string.Empty;

    public decimal Valor { get; set; }

    public DateTime Vencimento { get; set; }

    public bool Paga { get; set; }
}
