namespace CryptoQuote.Models.External.CryptoQuote;

public sealed record Currency
{
    [JsonPropertyName("price")]
    public decimal? Price { get; init; }

    [JsonPropertyName("last_updated")]
    public DateTime? LastUpdated { get; init; }
}