using ECommerceApp.Application.DTOs.Products;
using ECommerceApp.Application.Interfaces;
using ECommerceApp.Infrastructure.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ECommerceApp.Infrastructure.ExternalServices
{
    public class OpenAiProductDescriptionGenerator : IProductDescriptionGenerator
    {
        private readonly HttpClient _httpClient;
        private readonly OpenAISettings _settings;
        private readonly ILogger<OpenAiProductDescriptionGenerator> _logger;

        public OpenAiProductDescriptionGenerator(HttpClient httpClient, IOptions<OpenAISettings> options, ILogger<OpenAiProductDescriptionGenerator> logger)
        {
            _httpClient = httpClient;
            _settings = options.Value;
            _logger = logger;
        }

        public async Task<string> GenerateAsync(ProductDescriptionGenerationInput input, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(_settings.ApiKey))
                throw new InvalidOperationException("OpenAI ApiKey is not configured.");

            var prompt = BuildPrompt(input);

            var content = new List<object>
            {
                new
                {
                    type = "input_text",
                    text = prompt
                }
            };

            if (!string.IsNullOrWhiteSpace(input.ImageUrl))
            {
                content.Add(new
                {
                    type = "input_image",
                    image_url = input.ImageUrl,
                    detail = "low"
                });
            }

            var requestBody = new
            {
                model = _settings.Model,
                input = new object[]
                {
                    new
                    {
                        role = "user",
                        content
                    }
                },
                max_output_tokens = 300
            };

            using var request = new HttpRequestMessage(HttpMethod.Post, _settings.Endpoint);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _settings.ApiKey);
            request.Content = new StringContent(
                JsonSerializer.Serialize(requestBody),
                Encoding.UTF8,
                "application/json");

            using var response = await _httpClient.SendAsync(request, cancellationToken);
            var responseText = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("OpenAI request failed. StatusCode={StatusCode}, Response={Response}",
                    response.StatusCode, responseText);

                throw new InvalidOperationException("OpenAI product description generation failed.");
            }

            var description = ExtractOutputText(responseText);

            if (string.IsNullOrWhiteSpace(description))
                throw new InvalidOperationException("OpenAI returned an empty product description.");

            return description.Trim();
        }

        private static string BuildPrompt(ProductDescriptionGenerationInput input)
        {
            var sb = new StringBuilder();

            sb.AppendLine("Turkce, e-ticaret odakli bir urun aciklamasi yaz.");
            sb.AppendLine("Cikti sadece aciklama metni olsun.");
            sb.AppendLine("2-4 cumlelik, akici, profesyonel, ikna edici ama abartisiz olsun.");
            sb.AppendLine("HTML kullanma.");
            sb.AppendLine("Madde isareti kullanma.");
            sb.AppendLine("Gorsel net degilse emin olmadigin detaylari uydurma.");
            sb.AppendLine();

            sb.AppendLine($"Urun adi: {input.Name}");

            if (!string.IsNullOrWhiteSpace(input.CategoryName))
                sb.AppendLine($"Kategori: {input.CategoryName}");

            sb.AppendLine($"Fiyat: {input.Price}");

            if (!string.IsNullOrWhiteSpace(input.CurrentDescription))
                sb.AppendLine($"Mevcut aciklama: {input.CurrentDescription}");

            if (!string.IsNullOrWhiteSpace(input.ImageUrl))
                sb.AppendLine("Urun gorseli de verildi; gorunur ozellikleri dikkatli sekilde degerlendir.");

            return sb.ToString();
        }

        private static string ExtractOutputText(string json)
        {
            using var document = JsonDocument.Parse(json);

            if (!document.RootElement.TryGetProperty("output", out var outputArray) ||
                outputArray.ValueKind != JsonValueKind.Array)
            {
                return string.Empty;
            }

            var pieces = new List<string>();

            foreach (var outputItem in outputArray.EnumerateArray())
            {
                if (!outputItem.TryGetProperty("type", out var typeProp) ||
                    typeProp.GetString() != "message")
                {
                    continue;
                }

                if (!outputItem.TryGetProperty("content", out var contentArray) ||
                    contentArray.ValueKind != JsonValueKind.Array)
                {
                    continue;
                }

                foreach (var contentItem in contentArray.EnumerateArray())
                {
                    if (!contentItem.TryGetProperty("type", out var contentType) ||
                        contentType.GetString() != "output_text")
                    {
                        continue;
                    }

                    if (contentItem.TryGetProperty("text", out var textProp))
                    {
                        var text = textProp.GetString();
                        if (!string.IsNullOrWhiteSpace(text))
                            pieces.Add(text);
                    }
                }
            }

            return string.Join(Environment.NewLine, pieces);
        }
    }
}
