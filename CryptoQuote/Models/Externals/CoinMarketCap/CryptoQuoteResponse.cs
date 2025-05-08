namespace CryptoQuote.Models.External.CryptoQuote;

public sealed record CryptoQuoteResponse
{
    [JsonPropertyName("status")]
    public required Status Status { get; init; }

    [JsonPropertyName("data")]
    public Dictionary<string, Crypto[]>? Data { get; init; }
}