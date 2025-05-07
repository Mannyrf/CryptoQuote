namespace CryptoQuote.Dto.CryptoPrice;

public sealed record CryptoPriceResponse(string Name,
                                         string Symbol,
                                         IReadOnlyDictionary<string, decimal> Prices);
