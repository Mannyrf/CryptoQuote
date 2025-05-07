using CryptoQuote.Infrastructure.Configs;
using CryptoQuote.Models.External.ExchangeRates;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace CryptoQuote.Infrastructure.Externals.ExchangeRates;

public sealed class CachingExchangeRatesClientDecorator(
                    IExchangeRatesClient _exchangeRatesClient,
                    IOptions<ExchangeRatesConfigs> _exchangeRatesConfigs,
                    IMemoryCache _cache,
                    TimeProvider _timeProvider) : IExchangeRatesClient
{

    public async Task<ExchangeRatesResponse?> GetRatesAsync(string[] requestCurrencies, CancellationToken cancellationToken)
    {
        var cacheKey = $"exchange_rates_{string.Join(",", requestCurrencies.OrderBy(x => x))}";

        if (_cache.TryGetValue(cacheKey, out ExchangeRatesResponse? cachedResponse))
        {
            return cachedResponse;
        }

        var response = await _exchangeRatesClient.GetRatesAsync(requestCurrencies, cancellationToken);

        if (response != null)
        {
            _cache.Set(cacheKey, response, new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = _timeProvider.GetUtcNow().AddMinutes(_exchangeRatesConfigs.Value.CacheMinutes)
            });
        }

        return response;
    }
}