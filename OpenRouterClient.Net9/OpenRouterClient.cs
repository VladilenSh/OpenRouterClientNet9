using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
//using OpenRouterClient.Models;
using OpenRouterClientNet9.Models;

namespace OpenRouterClientNet9
{
    /// <summary>
    /// Клиент для работы с API OpenRouter
    /// </summary>
    public class OpenRouterClient : IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public OpenRouterClient(string apiKey, string? appName = null, string? siteUrl = null)
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://openrouter.ai/api/v1/"),
                DefaultRequestHeaders = { { "Authorization", $"Bearer {apiKey}" } }
            };

            if (!string.IsNullOrEmpty(appName))
                _httpClient.DefaultRequestHeaders.Add("X-Title", appName);

            if (!string.IsNullOrEmpty(siteUrl))
                _httpClient.DefaultRequestHeaders.Add("HTTP-Referer", siteUrl);

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
        }

        public async Task<ModelInfo[]> GetModelsAsync(CancellationToken ct = default)
        {
            var response = await _httpClient.GetFromJsonAsync<JsonDocument>("models", ct)
                ?? throw new InvalidOperationException("API returned null response.");

            var models = response.RootElement.GetProperty("data");
            return models.Deserialize<ModelInfo[]>(_jsonOptions)
                ?? throw new InvalidOperationException("Failed to deserialize models list.");
        }

        public async Task<ChatResponse> GenerateResponseAsync(
            string model,
            string prompt,
            double? temperature = null,
            int? maxTokens = null,
            CancellationToken ct = default)
        {
            var request = new ChatRequest(model, [new Message("user", prompt)], temperature, maxTokens);

            using var response = await _httpClient.PostAsJsonAsync(
                "chat/completions", request, _jsonOptions, ct);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<ChatResponse>(_jsonOptions, ct)
                ?? throw new InvalidOperationException("API returned null response.");
        }

        public void Dispose() => _httpClient.Dispose();
    }
}