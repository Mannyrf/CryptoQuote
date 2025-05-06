namespace CryptoQuote.Models.CryptoQuote;

public class CryptoQuoteResponse
{
    [JsonPropertyName("status")]
    public Status Status { get; set; }

    [JsonPropertyName("data")]
    public Dictionary<string, List<Crypto>> Data { get; set; }
}