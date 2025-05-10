using OpenRouterClientNet9; // Основной клиент OpenRouter
using OpenRouterClientNet9.Models; // Модели данных OpenRouter
using System;

// Получаем API ключ из переменных окружения
var apiKey = Environment.GetEnvironmentVariable("OPENROUTER_API_KEY")
    ?? throw new InvalidOperationException("OPENROUTER_API_KEY not set."); // Если ключ не найден, выбрасываем исключение

// Создаем клиент OpenRouter с указанием:
// - API ключа
// - Названия приложения (для идентификации в логах OpenRouter)
// - URL приложения (опционально)
using var client = new OpenRouterClient(apiKey, "OpenRouterClientNet9", "https://hd0.ru");

try
{
    // Запрашиваем список доступных моделей у OpenRouter
    var models = await client.GetModelsAsync();

    // Выводим информацию о моделях
    Console.WriteLine("Available models:");
    foreach (var model in models)
    {
        Console.WriteLine($"- {model.Name} \n 'Id' ({model.Id})");
        Console.WriteLine($"  Pricing: {model.Pricing.Prompt} (prompt) / {model.Pricing.Completion} (completion)\n");
    }

    Console.WriteLine($"\nВсего моделей: {models.Length}");

    // Отправляем запрос к конкретной модели (в данном случае deepseek-chat, бесплатная версия)
    var response = await client.GenerateResponseAsync(
        "deepseek/deepseek-chat:free", // Идентификатор модели
        "Привет! Расскажи о C# в двух предложениях."); // Текст запроса

    // Выводим полученный ответ
    //Console.WriteLine($"\nResponse: {response.Choices[0].Message.Content}");
    if (response.Choices.Length > 0 && !string.IsNullOrWhiteSpace(response.Choices[0].Message.Content))
    {
        Console.WriteLine($"\nОтвет модели: {response.Choices[0].Message.Content}");
    }
    else
    {
        Console.WriteLine("\nОтвет не был получен.");
    }


    // Выводим информацию об использовании токенов
    Console.WriteLine($"\nИспользовано токенов: \nОбщее: {response.Usage.TotalTokens}, \nPrompt: {response.Usage.PromptTokens}, \nCompletion: {response.Usage.CompletionTokens}");
}
catch (Exception ex)
{
    // Обрабатываем возможные ошибки (например, проблемы с сетью, невалидный API ключ и т.д.)
    Console.WriteLine($"Error: {ex.Message}");
}