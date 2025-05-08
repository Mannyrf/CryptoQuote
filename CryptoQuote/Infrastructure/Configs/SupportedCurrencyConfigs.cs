namespace CryptoQuote.Infrastructure.Configs;

public sealed record SupportedCurrencyConfigs
{
    public const string SectionName = "SupportedCurrency";

    [Required(AllowEmptyStrings = false), ValidateSupportedCurrencyConfigs(3, ["EUR"])]
    public required string[] Rates { get; set; }
}
