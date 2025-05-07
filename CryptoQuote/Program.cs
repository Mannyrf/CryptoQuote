using CryptoQuote.Endpoints;
using CryptoQuote.Infrastructure.Configs;
using CryptoQuote.Infrastructure.Externals.CoinMarketCap;
using CryptoQuote.Infrastructure.Externals.CryptoQuote;
using CryptoQuote.Infrastructure.Externals.ExchangeRates;
using CryptoQuote.Infrastructure.Http;
using CryptoQuote.Services.CryptoPriceService;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMemoryCache();
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddPolicyRegistry();

// Add services to the container.

builder.Services.AddOptions<CoinMarketCapConfigs>()
                .BindConfiguration(CoinMarketCapConfigs.SectionName)
                .ValidateDataAnnotations()
                .ValidateOnStart();

builder.Services.AddOptions<ExchangeRatesConfigs>()
                .BindConfiguration(ExchangeRatesConfigs.SectionName)
                .ValidateDataAnnotations()
                .ValidateOnStart();

builder.Services.AddOptions<SupportedCurrencyConfigs>()
                .BindConfiguration(SupportedCurrencyConfigs.SectionName)
                .ValidateDataAnnotations()
                .ValidateOnStart();

builder.Services.AddHttpClient<IExchangeRatesClient, ExchangeRatesClient>()
                .AddPolicyHandler(RetryPolicy.GetRetryPolicy())
                .AddPolicyHandler(CircuitBreakerPolicy.GetCircuitBreakerPolicy());

builder.Services.AddHttpClient<ICoinMarketCapClient, CoinMarketCapClient>()
                .AddPolicyHandler(RetryPolicy.GetRetryPolicy())
                .AddPolicyHandler(CircuitBreakerPolicy.GetCircuitBreakerPolicy());

builder.Services.Decorate<ICoinMarketCapClient, CachingCoinMarketCapClientDecorator>();
builder.Services.Decorate<IExchangeRatesClient, CachingExchangeRatesClientDecorator>();

builder.Services.AddScoped<ICryptoPriceService, CryptoPriceService>();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();
app.UseHttpsRedirection();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapCryptoPriceEndpoints();

app.Run();
