using CryptoQuote.Models.External.CryptoQuote;

namespace CryptoQuote.Infrastructure.Externals.CryptoQuote;

public interface ICoinMarketCapClient
{
    Task<CryptoQuoteResponse?> GetQuoteBySymbolAsync(string symbol, CancellationToken cancellationToken = default);
}