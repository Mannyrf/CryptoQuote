using CryptoQuote.Infrastructure.Configs;
using CryptoQuote.Infrastructure.Externals.CryptoQuote;
using CryptoQuote.Models.External.CryptoQuote;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace CryptoQuote.Infrastructure.Externals.CoinMarketCap;

public sealed class CachingCoinMarketCapClientDecorator(
                       ICoinMarketCapClient _coinMarketCapClient,
                       IOptions<CoinMarketCapConfigs> _coinMarketCapConfigs,
                       IMemoryCache _cache,
                       TimeProvider _timeProvider) : ICoinMarketCapClient
{

    public async Task<CryptoQuoteResponse?> GetQuoteBySymbolAsync(string symbol, CancellationToken cancellationToken)
    {
        var cacheKey = $"crypto_quote_{symbol.ToLowerInvariant()}";

        if (_cache.TryGetValue(cacheKey, out CryptoQuoteResponse? cachedResponse))
        {
            return cachedResponse;
        }

        var response = await _coinMarketCapClient.GetQuoteBySymbolAsync(symbol, cancellationToken);

        if (response != null)
        {
            _cache.Set(cacheKey, response, new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = _timeProvider.GetUtcNow().AddMinutes(_coinMarketCapConfigs.Value.CacheMinutes)
            });
        }

        return response;
    }
}