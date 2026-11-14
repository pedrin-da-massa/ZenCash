
namespace ZenCash.Api.Models;

public class ZennyChatRequest
{
    public string Mensagem { get; set; } = string.Empty;

    public List<string> Historico { get; set; } = new();
}
