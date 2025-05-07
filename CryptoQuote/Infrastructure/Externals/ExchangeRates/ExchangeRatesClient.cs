using CryptoQuote.Exceptions;
using CryptoQuote.Infrastructure.Configs;
using CryptoQuote.Models.External.ExchangeRates;
using CryptoQuote.Services.CryptoPriceService;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Http.Headers;

namespace CryptoQuote.Infrastructure.Externals.ExchangeRates;

public sealed class ExchangeRatesClient(HttpClient _httpClient,
                                        IOptions<ExchangeRatesConfigs> exchangeRatesConfigs,
                                        ILogger<CryptoPriceService> _logger) : IExchangeRatesClient
{
    const string AcceptableResponseType = "application/json";

    private readonly ExchangeRatesConfigs _exchangeRatesConfigs = exchangeRatesConfigs.Value;

    public async Task<ExchangeRatesResponse?> GetRatesAsync(string[] requestCurrencies, CancellationToken cancellationToken = default)
    {
        if (requestCurrencies is null || requestCurrencies.Length == 0)
        {
            throw new ArgumentException("Request currencies cannot be null or empty", nameof(requestCurrencies));
        }

        QueryString queryString = QueryString.Create("access_key", _exchangeRatesConfigs.ApiKey)
                                             .Add("symbols", string.Join(',', requestCurrencies))
                                             .Add("base", "EUR");
        UriBuilder uriBuilder = new(_exchangeRatesConfigs.BaseUrl)
        {
            Query = queryString.Value
        };

        _httpClient.DefaultRequestHeaders.Accept.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(AcceptableResponseType));

        JsonSerializerOptions _jsonSerializerOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase
        };

        try
        {
            _logger.LogDebug("Requesting currency rates");

            HttpResponseMessage response = await _httpClient.GetAsync(uriBuilder.Uri, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
                                        .ConfigureAwait(false);

            _logger.LogDebug("Received response with status: {StatusCode}", response.StatusCode);

            if (!response.IsSuccessStatusCode)
            {
                await HandleUnsuccessfulResponse(response, string.Join(',', requestCurrencies), cancellationToken);
            }

            return await response.Content.ReadFromJsonAsync<ExchangeRatesResponse>(_jsonSerializerOptions, cancellationToken)
                                        .ConfigureAwait(false);
        }
        catch (HttpRequestException ex) when (ex.StatusCode.HasValue)
        {
            _logger.LogError(ex, "API request failed with status {StatusCode} for {Currencies} currencies", ex.StatusCode, string.Join(',', requestCurrencies));
            throw new CryptoQuoteApiException($"API request failed with status code: {ex.StatusCode}", ex, ex.StatusCode.Value);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Network error while requesting quote for {Currencies} currencies", string.Join(',', requestCurrencies));
            throw new CryptoQuoteApiException("Network error occurred while accessing CoinMarketCap API", ex, null);
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogError(ex, "Request timeout for {Currencies} currencies", string.Join(',', requestCurrencies));
            throw new CryptoQuoteApiException("Request timeout occurred", ex, null);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to deserialize response");
            throw new CryptoQuoteApiException("Failed to process API response", ex, null);
        }
    }

    private async Task HandleUnsuccessfulResponse(HttpResponseMessage response, string symbol, CancellationToken cancellationToken)
    {
        try
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogError("API request failed with status {StatusCode}. Response: {ErrorContent}", response.StatusCode, errorContent);

            throw response.StatusCode switch
            {
                HttpStatusCode.Unauthorized => new CryptoQuoteApiException("Invalid API key or unauthorized access", null, response.StatusCode),
                HttpStatusCode.TooManyRequests => new CryptoQuoteApiException("Rate limit exceeded", null, response.StatusCode),
                HttpStatusCode.NotFound => new CryptoQuoteApiException($"Currency list not correct", null, response.StatusCode),
                _ => new CryptoQuoteApiException($"API request failed with status code: {response.StatusCode}", null, response.StatusCode)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to read error response content");
            throw new CryptoQuoteApiException($"API request failed with status code: {response.StatusCode}", ex, response.StatusCode);
        }
    }
}