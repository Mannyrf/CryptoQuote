namespace CryptoQuote.Configs;

public class ValidateSupportedCurrencyConfigs(int minLength) : ValidationAttribute
{
    public int? MinLength { get; } = minLength;
    public string[]? MandatoryCurrencies { get; init; }

    public ValidateSupportedCurrencyConfigs(int minLength, string[] mandatoryCurrency) : this(minLength)
    {
        MandatoryCurrencies = mandatoryCurrency;
    }

    public override bool IsValid(object value)
    {
        if (value is not IList<string> list || list.Count == 0)
        {
            return false;
        }

        foreach (var item in list)
        {
            if (string.IsNullOrWhiteSpace(item) || item.Length < MinLength)
            {
                return false;
            }
        }

        if (MandatoryCurrencies is not null && MandatoryCurrencies.Except(list).Any())
        {
            return false;
        }

        return true;
    }
}