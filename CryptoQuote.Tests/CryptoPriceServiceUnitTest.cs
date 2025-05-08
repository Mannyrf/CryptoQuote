using CryptoQuote.Exceptions;
using CryptoQuote.Infrastructure.Configs;
using CryptoQuote.Infrastructure.Externals.CryptoQuote;
using CryptoQuote.Infrastructure.Externals.ExchangeRates;
using CryptoQuote.Models.External.CryptoQuote;
using CryptoQuote.Models.External.ExchangeRates;
using CryptoQuote.Services.CryptoPriceService;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace CryptoQuote.Tests;

public class CryptoPriceServiceUnitTest
{
    private readonly Mock<ICoinMarketCapClient> _coinMarketCapClientMock;
    private readonly Mock<IExchangeRatesClient> _exchangeRatesClientMock;
    private readonly Mock<IOptionsMonitor<SupportedCurrencyConfigs>> _currencyConfigsMock;
    private readonly Mock<ILogger<CryptoPriceService>> _loggerMock;
    private readonly CryptoPriceService _service;

    public CryptoPriceServiceUnitTest()
    {
        _coinMarketCapClientMock = new Mock<ICoinMarketCapClient>();
        _exchangeRatesClientMock = new Mock<IExchangeRatesClient>();
        _currencyConfigsMock = new Mock<IOptionsMonitor<SupportedCurrencyConfigs>>();
        _loggerMock = new Mock<ILogger<CryptoPriceService>>();

        _currencyConfigsMock
            .Setup(c => c.CurrentValue)
            .Returns(new SupportedCurrencyConfigs { Rates = new[] { "USD", "EUR", "BRL" } });

        _service = new CryptoPriceService(
            _coinMarketCapClientMock.Object,
            _exchangeRatesClientMock.Object,
            _currencyConfigsMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task GetCryptoPriceInAcceptableCurrenciesAsync_ValidInputs_ReturnsExpectedResults()
    {
        // Arrange
        var symbol = "BTC";
        var cryptoQuoteResponse = new CryptoQuoteResponse
        {
            Status = new Status()
            {
                CreditCount = 1,
                Elapsed = 1,
                ErrorCode = 0
            },
            Data = new Dictionary<string, Crypto[]>
            {
                {
                    symbol, new[]
                    {
                        new Crypto
                        {
                            Name = "Bitcoin",
                            Symbol = "BTC",
                            IsActive = 1,
                            Quote = new Quote
                            {
                                EUR = new Currency { Price = 30000 }
                            }
                        }
                    }
                }
            }
        };

        var exchangeRatesResponse = new ExchangeRatesResponse
        {
            Rates = new Dictionary<string, decimal>
            {
                { "USD", 1.1m },
                { "EUR", 0.8m },
                { "BRL", 150m }
            }
        };

        _coinMarketCapClientMock
            .Setup(c => c.GetQuoteBySymbolAsync(symbol, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cryptoQuoteResponse);

        _exchangeRatesClientMock
            .Setup(c => c.GetRatesAsync(It.IsAny<string[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(exchangeRatesResponse);

        // Act
        var result = await _service.GetCryptoPriceInAcceptableCurrenciesAsync(symbol);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        var cryptoPrice = result.First();
        Assert.Equal("Bitcoin", cryptoPrice.Name);
        Assert.Equal("BTC", cryptoPrice.Symbol);
        Assert.Equal(3, cryptoPrice.Prices.Count);
        Assert.Equal(33000, cryptoPrice.Prices["USD"]);
        Assert.Equal(24000, cryptoPrice.Prices["EUR"]);
        Assert.Equal(4500000, cryptoPrice.Prices["BRL"]);
    }

    [Fact]
    public async Task GetCryptoPriceInAcceptableCurrenciesAsync_InvalidSymbol_ThrowsCryptoNotFoundException()
    {
        // Arrange
        var symbol = "INVALID";
        _coinMarketCapClientMock
            .Setup(c => c.GetQuoteBySymbolAsync(symbol, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CryptoQuoteResponse { Status = new Status(), Data = null });

        // Act & Assert
        await Assert.ThrowsAsync<CryptoNotFoundException>(() =>
            _service.GetCryptoPriceInAcceptableCurrenciesAsync(symbol));
    }

    [Fact]
    public async Task GetCryptoPriceInAcceptableCurrenciesAsync_NoExchangeRates_ThrowsExchangeRatesUnavailableException()
    {
        // Arrange
        var symbol = "BTC";
        var cryptoQuoteResponse = new CryptoQuoteResponse
        {
            Status = new Status()
            {
                CreditCount = 1,
                Elapsed = 1,
                ErrorCode = 0
            },
            Data = new Dictionary<string, Crypto[]>
            {
                {
                    symbol, new[]
                    {
                        new Crypto
                        {
                            Name = "Bitcoin",
                            Symbol = "BTC",
                            IsActive = 1,
                            Quote = new Quote
                            {
                                EUR = new Currency { Price = 30000 }
                            }
                        }
                    }
                }
            }
        };

        _coinMarketCapClientMock
            .Setup(c => c.GetQuoteBySymbolAsync(symbol, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cryptoQuoteResponse);

        _exchangeRatesClientMock
            .Setup(c => c.GetRatesAsync(It.IsAny<string[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ExchangeRatesResponse { Rates = null });

        // Act & Assert
        await Assert.ThrowsAsync<ExchangeRatesUnavailableException>(() =>
            _service.GetCryptoPriceInAcceptableCurrenciesAsync(symbol));
    }

    [Fact]
    public async Task GetCryptoPriceInAcceptableCurrenciesAsync_NullOrWhitespaceSymbol_ThrowsArgumentException()
    {
        // Arrange
        var symbol = " ";

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.GetCryptoPriceInAcceptableCurrenciesAsync(symbol));
    }
}
