namespace CryptoQuote.Models.External.CryptoQuote;

public sealed record Quote
{
    [JsonPropertyName("EUR")]
    public Currency? EUR { get; init; }
}
