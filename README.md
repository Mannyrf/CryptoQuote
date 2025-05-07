# CryptoQuote

CryptoQuote is a .NET 9 application designed to fetch and provide cryptocurrency price data. It integrates with external APIs, such as CoinMarketCap, to deliver accurate and up-to-date information. The project is structured with a focus on clean architecture, testability, and scalability.

## Features

- Fetch real-time cryptocurrency prices.
- RESTful API endpoints for querying cryptocurrency data.
- Integration with external services like CoinMarketCap.
- Unit tests to ensure reliability and maintainability.
- Built with .NET 9 for modern performance and features.

## Getting Started

Follow these instructions to set up and run the project locally.

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- A [CoinMarketCap](https://pro.coinmarketcap.com/account) API key.
- A [ExchangeRatesApi](https://manage.exchangeratesapi.io/) API key.

### Installation

1. Clone the repository:
   git clone https://github.com/Mannyrf/CryptoQuote.git cd CryptoQuote

2. Restore dependencies:
	dotnet restore
 
3. Build the project:
	dotnet build

4. Set up environment variables for API keys:
- Set CoinMarketCap API key in appsettings.Development.json file
- Set ExchangeRatesApi API key in appsettings.Development.json file
- Change supported currensies in appsettings.Development.json, SupportedCurrency key. default values are
`EUR,USD,BRL,GBP,AUD`
    
### Running the Application

To run the application locally:

	dotnet run --project CryptoQuote

The application will start and expose its API endpoints. By default, it runs on `https://localhost:7272`.

### Running Tests

To execute the unit tests:

	dotnet test

The project uses `xUnit` for testing, along with `Moq` for mocking dependencies.

## API Endpoints

The application provides the following endpoints:

### `GET /api/quotes/{symbol}`
Fetch the current price of a cryptocurrency.

#### Query Parameters:
- `symbol` (required): The symbol of the cryptocurrency (e.g., BTC, ETH).

#### Example Request:

    curl "https://localhost:7272/api/quotes/BTC"

#### Example Response:

```json
[
  {
    "name": "Toncoin",
    "symbol": "TON",
    "prices": {
      "EUR": 2.68,
      "USD": 3.03,
      "BRL": 17.41,
      "GBP": 2.28,
      "AUD": 4.72
    }
  },
  {
    "name": "TON Token",
    "symbol": "TON",
    "prices": {
      "EUR": 0.461974,
      "USD": 0.522353,
      "BRL": 3,
      "GBP": 0.392976,
      "AUD": 0.812752
    }
  }
]
```

## Project Structure

The project follows a modular structure:

- **Endpoints**: API endpoint definitions.
- **Dto**: Data Transfer Objects for request and response models.
- **Services**: Core business logic.
- **Infrastructure**: External API integrations (e.g., CoinMarketCap).
- **Tests**: Unit tests for services and other components.

## Technologies Used

- **.NET 9**: Modern, high-performance framework.
- **xUnit**: Unit testing framework.
- **Moq**: Mocking library for unit tests.
- **CoinMarketCap API**: External service for cryptocurrency data.

## Contributing

Contributions are welcome! Please follow these steps:

1. Fork the repository.
2. Create a new branch for your feature or bugfix:
   
	git checkout -b feature/feature-name

3. Make your changes and commit them:
   
	git commit -m "Add my feature"
      
4. Push to your branch:
   
	git push origin feature-name

5. Open a pull request.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

## Acknowledgments

- [CoinMarketCap](https://coinmarketcap.com/) for cryptocurrency data.
- [ExchangeRatesApi](https://exchangeratesapi.io/) for currency rate data.
- The .NET community for their support and tools.

---

Feel free to customize this README to better suit your project's needs.
