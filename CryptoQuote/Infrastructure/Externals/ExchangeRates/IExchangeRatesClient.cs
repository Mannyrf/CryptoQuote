using CryptoQuote.Models.External.ExchangeRates;

namespace CryptoQuote.Infrastructure.Externals.ExchangeRates;

public interface IExchangeRatesClient
{
    Task<ExchangeRatesResponse?> GetRatesAsync(string[] requestCurrencies, CancellationToken cancellationToken = default);
}
