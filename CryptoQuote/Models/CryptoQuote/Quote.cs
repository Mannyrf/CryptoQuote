namespace CryptoQuote.Models.CryptoQuote;

public class Quote
{
    [JsonPropertyName("USD")]
    public Currency? Usd { get; set; }
}
