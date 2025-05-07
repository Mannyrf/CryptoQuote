using Polly;
using Polly.Extensions.Http;

namespace CryptoQuote.Infrastructure.Http;

public static class RetryPolicy
{
    private const int MaxRetries = 3;
    public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return HttpPolicyExtensions.HandleTransientHttpError()
                                   .OrResult(msg => !msg.IsSuccessStatusCode)
                                   .WaitAndRetryAsync(MaxRetries, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
    }
}