using OpenRouterClientNet9.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OpenRouterClientNet9.Models
{
    internal class ChatResponse
    {
    }
}
namespace OpenRouterClientNet9
{
    /// <summary>
    /// Ответ от API чата
    /// </summary>
    public record ChatResponse(
        [property: JsonPropertyName("choices")] Choice[] Choices,
        [property: JsonPropertyName("usage")] Usage Usage);

    /// <summary>
    /// Вариант ответа модели
    /// </summary>
    public record Choice(
        [property: JsonPropertyName("message")] Message Message,
        [property: JsonPropertyName("finish_reason")] string FinishReason);

    /// <summary>
    /// Использованные токены
    /// </summary>
    public record Usage(
        [property: JsonPropertyName("prompt_tokens")] int PromptTokens,
        [property: JsonPropertyName("completion_tokens")] int CompletionTokens,
        [property: JsonPropertyName("total_tokens")] int TotalTokens);
}