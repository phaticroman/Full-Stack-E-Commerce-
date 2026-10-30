using MaleFashion.Application.Contracts.Services;
using MaleFashion.Domain.Utilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace MaleFashion.Infrastructure.Services
{
    public class RecaptchaService : IRecaptchaService
    {
        private const double MinimumScore = 0.5;

        private readonly HttpClient _httpClient;
        private readonly RecaptchaSettings _settings;
        private readonly ILogger<RecaptchaService> _logger;

        public RecaptchaService(HttpClient httpClient, IOptions<RecaptchaSettings> settings, ILogger<RecaptchaService> logger)
        {
            _httpClient = httpClient;
            _settings = settings.Value;
            _logger = logger;
        }

        public async Task<bool> VerifyAsync(string? token, string expectedAction, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                _logger.LogWarning("reCAPTCHA verify called with empty token.");
                return false;
            }

            var response = await _httpClient.PostAsync(
                $"https://www.google.com/recaptcha/api/siteverify?secret={_settings.SecretKey}&response={token}",
                null, cancellationToken);

            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogInformation("reCAPTCHA raw response: {Json}", json);

            if (!response.IsSuccessStatusCode)
                return false;

            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            var success = root.TryGetProperty("success", out var successProp) && successProp.GetBoolean();
            if (!success)
                return false;

            if (root.TryGetProperty("action", out var actionProp) &&
                !string.Equals(actionProp.GetString(), expectedAction, StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning("reCAPTCHA action mismatch. Expected '{Expected}', got '{Actual}'.",
                    expectedAction, actionProp.GetString());
                return false;
            }

            if (root.TryGetProperty("score", out var scoreProp))
                return scoreProp.GetDouble() >= MinimumScore;

            return false;
        }
    }
}
