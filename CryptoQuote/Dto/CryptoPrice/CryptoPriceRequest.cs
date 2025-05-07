namespace CryptoQuote.Dto.CryptoPrice;

public sealed record CryptoPriceRequest
{
    public required string Symbol { get; set; }
}
