namespace CryptoQuote.Models.External.CryptoQuote;

public sealed record Status
{
    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; init; }

    [JsonPropertyName("error_code")]
    public int ErrorCode { get; init; }

    [JsonPropertyName("error_message")]
    public object ErrorMessage { get; init; }

    [JsonPropertyName("elapsed")]
    public int Elapsed { get; init; }

    [JsonPropertyName("credit_count")]
    public int CreditCount { get; init; }

    [JsonPropertyName("notice")]
    public object Notice { get; init; }
}