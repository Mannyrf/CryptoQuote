namespace CryptoQuote.Models.External.ExchangeRates;

public sealed record ExchangeRatesResponse
{
    [JsonPropertyName("success")]
    public bool IsSuccess { get; init; }

    [JsonPropertyName("timestamp")]
    public int Timestamp { get; init; }

    [JsonPropertyName("base")]
    public string BaseCurrency { get; init; }

    [JsonPropertyName("date")]
    public string Date { get; init; }

    [JsonPropertyName("rates")]
    public Dictionary<string, decimal> Rates { get; init; }
}