using CryptoQuote.Dto.CryptoPrice;
using CryptoQuote.Exceptions;
using CryptoQuote.Infrastructure.Configs;
using CryptoQuote.Infrastructure.Externals.CryptoQuote;
using CryptoQuote.Infrastructure.Externals.ExchangeRates;
using CryptoQuote.Models.External.CryptoQuote;
using CryptoQuote.Models.External.ExchangeRates;
using Microsoft.Extensions.Options;
using System.Collections.Immutable;

namespace CryptoQuote.Services.CryptoPriceService;

public class CryptoPriceService : ICryptoPriceService
{
    private readonly ICoinMarketCapClient _coinMarketCapClient;
    private readonly IExchangeRatesClient _exchangeRatesClient;
    private readonly IOptionsMonitor<SupportedCurrencyConfigs> _currencyConfigs;
    private readonly ILogger<CryptoPriceService> _logger;

    public CryptoPriceService(ICoinMarketCapClient coinMarketCapClient,
                              IExchangeRatesClient exchangeRatesClient,
                              IOptionsMonitor<SupportedCurrencyConfigs> currencyConfigs,
                              ILogger<CryptoPriceService> logger)
    {
        _coinMarketCapClient = coinMarketCapClient;
        _exchangeRatesClient = exchangeRatesClient;
        _currencyConfigs = currencyConfigs;
        _logger = logger;
    }

    public async Task<IReadOnlyCollection<CryptoPriceResponse>> GetCryptoPriceInAcceptableCurrenciesAsync(string symbol, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(symbol);

        var cryptoQuoteTask = _coinMarketCapClient.GetQuoteBySymbolAsync(symbol, cancellationToken);
        var exchangeRatesTask = _exchangeRatesClient.GetRatesAsync(_currencyConfigs.CurrentValue.Rates, cancellationToken);

        await Task.WhenAll(cryptoQuoteTask, exchangeRatesTask);

        var cryptoQuote = await cryptoQuoteTask;
        var exchangeRates = await exchangeRatesTask;

        if (cryptoQuote?.Data?.FirstOrDefault() is not { Value: var cryptos } || cryptos.Length == 0)
        {
            _logger.LogWarning("Crypto symbol {Symbol} is not available", symbol);
            throw new CryptoNotFoundException(symbol);
        }

        if (exchangeRates?.Rates is null || exchangeRates.Rates.Count == 0)
        {
            _logger.LogWarning("Exchange rates not available");
            throw new ExchangeRatesUnavailableException();
        }

        return ConvertToTargetCurrencies(cryptos, exchangeRates).ToImmutableArray();
    }

    private IEnumerable<CryptoPriceResponse> ConvertToTargetCurrencies(Crypto[] cryptoQuote, ExchangeRatesResponse exchangeRates)
    {
        const byte smallPriceDecimals = 6;
        const byte bigPriceDecimals = 2;

        foreach (var crypto in cryptoQuote)
        {
            // Skip deactivated crypto
            if (crypto.IsActive != 1)
            {
                continue;
            }

            // Skip null or zero prices
            if (crypto.Quote?.EUR?.Price is not decimal cryptoPrice || cryptoPrice <= 0)
            {
                continue;
            }

            var prices = new Dictionary<string, decimal>();

            foreach (var rate in exchangeRates.Rates.Distinct())
            {
                decimal priceInTargetCurrency = rate.Value * cryptoPrice;

                decimal roundedPrice = priceInTargetCurrency = priceInTargetCurrency >= 1
                     ? Math.Round(priceInTargetCurrency, bigPriceDecimals)
                     : Math.Round(priceInTargetCurrency, smallPriceDecimals);

                // return all digits if rounded price is less than smallPriceDecimals
                roundedPrice = roundedPrice == 0 ? priceInTargetCurrency : roundedPrice;

                prices[rate.Key] = roundedPrice;
            }

            if (prices.Count > 0)
            {
                yield return new(
                            crypto.Name ?? string.Empty,
                            crypto.Symbol ?? string.Empty,
                            prices.ToImmutableDictionary());
            }
        }
    }
}
