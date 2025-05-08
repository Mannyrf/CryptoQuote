namespace CryptoQuote.Models.External.CryptoQuote;

public sealed record Crypto
{
    [JsonPropertyName("id")]
    public long? Id { get; init; }

    [JsonPropertyName("name")]
    public string? Name { get; init; }

    [JsonPropertyName("symbol")]
    public string? Symbol { get; init; }

    [JsonPropertyName("slug")]
    public string? Slug { get; init; }

    [JsonPropertyName("is_active")]
    public int? IsActive { get; init; }

    [JsonPropertyName("cmc_rank")]
    public long? CmcRank { get; init; }

    [JsonPropertyName("last_updated")]
    public DateTime? LastUpdated { get; init; }

    [JsonPropertyName("quote")]
    public Quote? Quote { get; init; }
}
