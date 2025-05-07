using CryptoQuote.Exceptions;
using CryptoQuote.Infrastructure.Configs;
using CryptoQuote.Infrastructure.Externals.CryptoQuote;
using CryptoQuote.Models.External.CryptoQuote;
using CryptoQuote.Services.CryptoPriceService;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Http.Headers;

namespace CryptoQuote.Infrastructure.Externals.CoinMarketCap;

public sealed class CoinMarketCapClient : ICoinMarketCapClient
{
    const string AcceptableResponseType = "application/json";

    private readonly HttpClient _httpClient;
    private readonly CoinMarketCapConfigs _coinMarketCapConfigs;
    private readonly JsonSerializerOptions _jsonSerializerOptions;
    private readonly ILogger<CryptoPriceService> _logger;

    public CoinMarketCapClient(HttpClient httpClient,
                               IOptions<CoinMarketCapConfigs> coinMarketCapConfigs,
                               ILogger<CryptoPriceService> logger)
    {
        _httpClient = httpClient;
        _coinMarketCapConfigs = coinMarketCapConfigs.Value;
        _logger = logger;

        _jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase
        };
    }

    public async Task<CryptoQuoteResponse?> GetQuoteBySymbolAsync(string symbol, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(symbol);

        QueryString queryString = QueryString.Create("symbol", symbol.ToLowerInvariant())
                                             .Add("convert", "EUR");

        UriBuilder uriBuilder = new(_coinMarketCapConfigs.BaseUrl)
        {
            Query = queryString.Value
        };

        _httpClient.DefaultRequestHeaders.Accept.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(AcceptableResponseType));
        _httpClient.DefaultRequestHeaders.Add("X-CMC_PRO_API_KEY", _coinMarketCapConfigs.ApiKey);

        try
        {
            _logger.LogDebug("Requesting crypto quote for symbol: {Symbol}", symbol);

            using HttpResponseMessage response = await _httpClient.GetAsync(uriBuilder.Uri, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
                                                                 .ConfigureAwait(false);

            _logger.LogDebug("Received response with status: {StatusCode}", response.StatusCode);

            if (!response.IsSuccessStatusCode)
            {
                await HandleUnsuccessfulResponse(response, symbol, cancellationToken);
            }

            return await response.Content.ReadFromJsonAsync<CryptoQuoteResponse>(_jsonSerializerOptions, cancellationToken)
                                        .ConfigureAwait(false);
        }
        catch (HttpRequestException ex) when (ex.StatusCode.HasValue)
        {
            _logger.LogError(ex, "API request failed with status {StatusCode} for symbol {Symbol}",
                ex.StatusCode, symbol);
            throw new CryptoQuoteApiException($"API request failed with status code: {ex.StatusCode}", ex, ex.StatusCode.Value);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Network error while requesting quote for symbol {Symbol}", symbol);
            throw new CryptoQuoteApiException("Network error occurred while accessing CoinMarketCap API", ex, null);
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogError(ex, "Request timeout for symbol {Symbol}", symbol);
            throw new CryptoQuoteApiException("Request timeout occurred", ex, null);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to deserialize response for symbol {Symbol}", symbol);
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
                HttpStatusCode.NotFound => new CryptoQuoteApiException($"Symbol '{symbol}' not found", null, response.StatusCode),
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