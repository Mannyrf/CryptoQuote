using CryptoQuote.Dto.CryptoPrice;

namespace CryptoQuote.Services.CryptoPriceService;

public interface ICryptoPriceService
{
    Task<IReadOnlyCollection<CryptoPriceResponse>> GetCryptoPriceInAcceptableCurrenciesAsync(string symbol, CancellationToken cancellationToken = default);
}
