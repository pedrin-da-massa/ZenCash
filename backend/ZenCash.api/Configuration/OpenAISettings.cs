
namespace ZenCash.Api.Configuration;

public class OpenAISettings
{
    public string ApiKey { get; set; } = string.Empty;

    public string Model { get; set; } = string.Empty;

    public string BaseUrl { get; set; } =
        "https://api.openai.com/v1/responses";
}
