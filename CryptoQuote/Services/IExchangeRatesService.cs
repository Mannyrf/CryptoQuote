namespace CryptoQuote.Services;

public interface IExchangeRatesService
{
    Task<string> GetPriceAsync();
}
