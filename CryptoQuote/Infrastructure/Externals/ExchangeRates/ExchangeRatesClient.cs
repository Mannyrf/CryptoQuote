using CryptoQuote.Infrastructure.Configs;
using CryptoQuote.Models.External.ExchangeRates;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;

namespace CryptoQuote.Infrastructure.Externals.ExchangeRates;

public class ExchangeRatesClient(HttpClient httpClient, IOptions<ExchangeRatesConfigs> exchangeRatesConfigs) : IExchangeRatesClient
{
    const string AcceptableResponseType = "application/json";

    private readonly HttpClient _httpClient = httpClient;
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

        JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase
        };

        try
        {
            HttpResponseMessage response = await _httpClient.GetAsync(uriBuilder.Uri, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
                                        .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<ExchangeRatesResponse>(_jsonSerializerOptions, cancellationToken)
                                        .ConfigureAwait(false);
        }
        catch
        {
            throw;
        }
    }
}