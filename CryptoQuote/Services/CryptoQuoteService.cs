using CryptoQuote.Models.CryptoQuote;
using Microsoft.Extensions.Options;
using System.Net;
using System.Web;

namespace CryptoQuote.Services;

public sealed class CryptoQuoteService : ICryptoQuoteService
{
    private readonly CoinMarketCapConfigs _coinMarketCapConfigs;

    public CryptoQuoteService(IOptions<CoinMarketCapConfigs> coinMarketCapConfigs)
    {
        _coinMarketCapConfigs = coinMarketCapConfigs.Value;
    }

    public async Task<CryptoQuoteResponse> GetQuoteAsync(string symbol)
    {
        var URL = new UriBuilder(_coinMarketCapConfigs.BaseUrl);

        var queryString = HttpUtility.ParseQueryString(string.Empty);
        //queryString["start"] = "1";
        //queryString["limit"] = "10";
        //queryString["convert"] = "USD";
        queryString["symbol"] = symbol.ToLower();

        URL.Query = queryString.ToString();

        var client = new WebClient();
        client.Headers.Add("X-CMC_PRO_API_KEY", _coinMarketCapConfigs.ApiKey);
        client.Headers.Add("Accepts", "application/json");
        string response = client.DownloadString(URL.ToString());

        var result = JsonSerializer.Deserialize<CryptoQuoteResponse>(response, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return result;
    }
}

