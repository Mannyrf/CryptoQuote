using Microsoft.Extensions.Options;
using System.Net;
using System.Web;

namespace CryptoQuote.Services;

public class ExchangeRatesService : IExchangeRatesService
{
    private readonly ExchangeRatesConfigs _exchangeRatesConfigs;
    private readonly SupportedCurrencyConfigs _supportedCurrencyConfigs;

    public ExchangeRatesService(IOptions<ExchangeRatesConfigs> exchangeRatesConfigs ,IOptionsMonitor<SupportedCurrencyConfigs> supportedCurrencyConfigs)
    {
        _exchangeRatesConfigs = exchangeRatesConfigs.Value;
        _supportedCurrencyConfigs = supportedCurrencyConfigs.CurrentValue;
    }

    public async Task<string> GetPriceAsync()
    {
        var URL = new UriBuilder(_exchangeRatesConfigs.BaseUrl);

        var queryString = HttpUtility.ParseQueryString(string.Empty);
        queryString["access_key"] = _exchangeRatesConfigs.ApiKey;
        queryString["symbols"] = string.Join(',', _supportedCurrencyConfigs.Rates);

        URL.Query = queryString.ToString();

        var client = new WebClient();
        client.Headers.Add("Accepts", "application/json");

        string result = client.DownloadString(URL.ToString());

        return result;
    }


}
