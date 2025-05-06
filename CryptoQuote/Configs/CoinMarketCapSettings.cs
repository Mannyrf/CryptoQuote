namespace CryptoQuote.Configs;

public sealed record CoinMarketCapConfigs
{
    public const string SectionName = "CoinMarketCap";

    [Required]
    public required string ApiKey { get; init; }

    [Url]
    public required string BaseUrl { get; init; }
}
