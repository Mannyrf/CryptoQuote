namespace CryptoQuote.Models.CryptoQuote;

public class Currency
{
    [JsonPropertyName("price")]
    public decimal? Price { get; set; }

    [JsonPropertyName("volume_24h")]
    public decimal? Volume24H { get; set; }

    [JsonPropertyName("volume_change_24h")]
    public decimal? VolumeChange24H { get; set; }

    [JsonPropertyName("percent_change_1h")]
    public decimal? PercentChange1H { get; set; }

    [JsonPropertyName("percent_change_24h")]
    public decimal? PercentChange24H { get; set; }

    [JsonPropertyName("percent_change_7d")]
    public decimal? PercentChange7D { get; set; }

    [JsonPropertyName("percent_change_30d")]
    public decimal? PercentChange30D { get; set; }

    [JsonPropertyName("percent_change_60d")]
    public decimal? PercentChange60D { get; set; }

    [JsonPropertyName("percent_change_90d")]
    public decimal? PercentChange90D { get; set; }

    [JsonPropertyName("market_cap")]
    public decimal? MarketCap { get; set; }

    [JsonPropertyName("market_cap_dominance")]
    public decimal? MarketCapDominance { get; set; }

    [JsonPropertyName("fully_diluted_market_cap")]
    public decimal? FullyDilutedMarketCap { get; set; }

    [JsonPropertyName("tvl")]
    public object? Tvl { get; set; }

    [JsonPropertyName("last_updated")]
    public DateTime? LastUpdated { get; set; }
}