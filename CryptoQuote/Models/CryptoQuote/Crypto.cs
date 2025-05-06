
namespace CryptoQuote.Models.CryptoQuote;

public class Crypto
{
    [JsonPropertyName("id")]
    public int? Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("symbol")]
    public string Symbol { get; set; }

    [JsonPropertyName("slug")]
    public string Slug { get; set; }

    [JsonPropertyName("num_market_pairs")]
    public int? NumMarketPairs { get; set; }

    [JsonPropertyName("date_added")]
    public DateTime? DateAdded { get; set; }

    [JsonPropertyName("max_supply")]
    public long? MaxSupply { get; set; }

    [JsonPropertyName("circulating_supply")]
    public int? CirculatingSupply { get; set; }

    [JsonPropertyName("total_supply")]
    public long? TotalSupply { get; set; }

    [JsonPropertyName("is_active")]
    public int IsActive { get; set; } = 0;

    [JsonPropertyName("infinite_supply")]
    public bool InfiniteSupply { get; set; } = false;

    [JsonPropertyName("cmc_rank")]
    public int? CmcRank { get; set; }

    [JsonPropertyName("is_fiat")]
    public int? IsFiat { get; set; }

    [JsonPropertyName("self_reported_circulating_supply")]
    public long? SelfReportedCirculatingSupply { get; set; }

    [JsonPropertyName("self_reported_market_cap")]
    public decimal? SelfReportedMarketCap { get; set; }

    [JsonPropertyName("tvl_ratio")]
    public object? TvlRatio { get; set; }

    [JsonPropertyName("last_updated")]
    public DateTime? LastUpdated { get; set; }

    [JsonPropertyName("quote")]
    public Quote? Quote { get; set; }
}
