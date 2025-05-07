using System.Net;

namespace CryptoQuote.Exceptions;

public class CryptoQuoteApiException : HttpRequestException
{
    public string? ErrorDetails { get; }

    public CryptoQuoteApiException(string message, Exception? innerException, HttpStatusCode? statusCode) : base(message, innerException, statusCode)
    {
    }

    public CryptoQuoteApiException(string message, string errorDetails, Exception? innerException, HttpStatusCode? statusCode) : base(message, innerException, statusCode)
    {
        ErrorDetails = errorDetails;
    }
}