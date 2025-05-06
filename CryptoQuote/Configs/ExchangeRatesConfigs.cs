namespace CryptoQuote.Configs;

public sealed record ExchangeRatesConfigs
{
    public const string SectionName = "ExchangeRates";

    [Required]
    public required string ApiKey { get; init; }

    [Url]
    public required string BaseUrl { get; init; }
}
