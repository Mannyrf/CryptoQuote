using CryptoQuote.Dto.CryptoPrice;
using CryptoQuote.Services.CryptoPriceService;

namespace CryptoQuote.Endpoints;

public static class CryptoPriceEndpoints
{
    public static void MapCryptoPriceEndpoints(this WebApplication app)
    {
        app.MapGet("/api/quotes/{symbol}",
            async ([AsParameters] CryptoPriceRequest request, ICryptoPriceService service) =>
            {
                return Results.Ok(await service.GetCryptoPriceInAcceptableCurrenciesAsync(request.Symbol));
            })
            .AddEndpointFilter(async (invocationContext, next) =>
            {
                CryptoPriceRequest symbol = invocationContext.GetArgument<CryptoPriceRequest>(0);

                if (string.IsNullOrWhiteSpace(symbol.Symbol) ||
                   !symbol.Symbol.All(x => char.IsLetter(x)) ||
                   symbol.Symbol.Length < 3)
                {
                    return Results.Problem("Symbol format is not correct.");
                }

                return await next(invocationContext);
            });
    }
}
