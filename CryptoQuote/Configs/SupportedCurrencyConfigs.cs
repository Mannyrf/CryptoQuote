namespace CryptoQuote.Configs;

public sealed record SupportedCurrencyConfigs
{
    public const string SectionName = "SupportedCurrency";

    [Required(AllowEmptyStrings = false), ValidateSupportedCurrencyConfigs(3, ["USD"])]
    public required IList<string> Rates { get; set; }
}
