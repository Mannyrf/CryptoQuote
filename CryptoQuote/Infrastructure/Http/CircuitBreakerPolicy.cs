using Polly;
using Polly.Extensions.Http;

namespace CryptoQuote.Infrastructure.Http;

public static class CircuitBreakerPolicy
{
    private const int eventAllowded = 5;
    private const int waitSeconds = 30;

    public static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
    {
        return HttpPolicyExtensions.HandleTransientHttpError()
                                   .CircuitBreakerAsync(eventAllowded, TimeSpan.FromSeconds(waitSeconds));
    }
}