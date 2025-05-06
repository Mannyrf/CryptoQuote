using CryptoQuote.Models.CryptoQuote;

namespace CryptoQuote.Services;

public interface ICryptoQuoteService
{
    Task<CryptoQuoteResponse> GetQuoteAsync(string symbol);
}