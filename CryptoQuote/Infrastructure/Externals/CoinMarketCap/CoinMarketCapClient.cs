using CryptoQuote.Infrastructure.Configs;
using CryptoQuote.Models.External.CryptoQuote;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;

namespace CryptoQuote.Infrastructure.Externals.CryptoQuote;

public sealed class CoinMarketCapClient : ICoinMarketCapClient
{
    const string AcceptableResponseType = "application/json";

    private readonly HttpClient _httpClient;
    private readonly CoinMarketCapConfigs _coinMarketCapConfigs;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public CoinMarketCapClient(HttpClient httpClient, IOptions<CoinMarketCapConfigs> coinMarketCapConfigs)
    {
        _httpClient = httpClient;
        _coinMarketCapConfigs = coinMarketCapConfigs.Value;

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
            HttpResponseMessage response = await _httpClient.GetAsync(uriBuilder.Uri, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
                                                            .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<CryptoQuoteResponse>(_jsonSerializerOptions, cancellationToken)
                                        .ConfigureAwait(false);
        }
        catch
        {
            throw;
        }
    }
}