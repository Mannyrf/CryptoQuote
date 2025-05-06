using CryptoQuote.Services;

var builder = WebApplication.CreateBuilder(args);

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

builder.Services.AddScoped<ICryptoQuoteService, CryptoQuoteService>();
builder.Services.AddScoped<IExchangeRatesService, ExchangeRatesService>();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGet("/api/quotes/{symbol}",
    async (string symbol, ICryptoQuoteService service) =>
    {
        return Results.Ok(await service.GetQuoteAsync(symbol));
    });

app.MapGet("/api/rates",
    async (IExchangeRatesService service) =>
    {
        return Results.Ok(await service.GetPriceAsync());
    });

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
