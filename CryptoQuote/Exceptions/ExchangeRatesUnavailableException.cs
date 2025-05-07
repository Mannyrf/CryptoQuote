namespace CryptoQuote.Exceptions;

public sealed class ExchangeRatesUnavailableException : Exception
{
    public ExchangeRatesUnavailableException()
        : base("Exchange rates service is currently unavailable")
    {
    }

    public ExchangeRatesUnavailableException(Exception innerException)
        : base("Exchange rates service is currently unavailable", innerException)
    {
    }
}