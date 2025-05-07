namespace CryptoQuote.Exceptions;

public sealed class CryptoNotFoundException : Exception
{
    public string Symbol { get; }

    public CryptoNotFoundException(string symbol) : base($"Cryptocurrency with symbol '{symbol}' was not found")
    {
        Symbol = symbol;
    }

    public CryptoNotFoundException(string symbol, Exception innerException) : base($"Cryptocurrency with symbol '{symbol}' was not found", innerException)
    {
        Symbol = symbol;
    }
}