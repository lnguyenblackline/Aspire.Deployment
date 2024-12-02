using Dapper;
using Npgsql;

namespace Aspire.Deployment.ApiService;

internal sealed class DatabaseInitializer(
    NpgsqlDataSource dataSource,
    IConfiguration configuration,
    ILogger<DatabaseInitializer> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            logger.LogInformation("Starting database initialization.");

            await EnsureDatabaseExists();
            await InitializeDatabase();
            await SeedInitialData();

            logger.LogInformation("Database initialization completed successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while initializing the database.");
        }
    }

    private async Task EnsureDatabaseExists()
    {
        string connectionString = configuration.GetConnectionString("stocks")!;
        var builder = new NpgsqlConnectionStringBuilder(connectionString);
        string? databaseName = builder.Database;
        builder.Database = "postgres"; // Connect to the default 'postgres' database

        using var connection = new NpgsqlConnection(builder.ToString());
        await connection.OpenAsync();

        bool databaseExists = await connection.ExecuteScalarAsync<bool>(
            "SELECT EXISTS(SELECT 1 FROM pg_database WHERE datname = @databaseName)",
            new { databaseName });

        if (!databaseExists)
        {
            logger.LogInformation("Creating database {DatabaseName}", databaseName);
            await connection.ExecuteAsync($"CREATE DATABASE {databaseName}");
        }
    }

    private async Task InitializeDatabase()
    {
        const string sql =
            """
            -- Check if the table exists, if not, create it
            CREATE TABLE IF NOT EXISTS public.stock_prices (
                id SERIAL PRIMARY KEY,
                ticker VARCHAR(10) NOT NULL,
                price NUMERIC(12, 6) NOT NULL,
                timestamp TIMESTAMP WITHOUT TIME ZONE DEFAULT (NOW() AT TIME ZONE 'UTC')
            );

            -- Create an index on the ticker column for faster lookups
            CREATE INDEX IF NOT EXISTS idx_stock_prices_ticker ON public.stock_prices(ticker);

            -- Create an index on the timestamp column for faster time-based queries
            CREATE INDEX IF NOT EXISTS idx_stock_prices_timestamp ON public.stock_prices(timestamp);
            """;
        using NpgsqlConnection connection = await dataSource.OpenConnectionAsync();
        await connection.ExecuteAsync(sql);
    }

    private async Task SeedInitialData()
    {
        using NpgsqlConnection connection = await dataSource.OpenConnectionAsync();

        // Check if the stock_prices table is empty
        bool isEmpty = await connection.ExecuteScalarAsync<bool>(
            "SELECT COUNT(*) = 0 FROM public.stock_prices"
        );

        if (!isEmpty)
        {
            return;
        }

        logger.LogInformation("Seeding initial stock prices.");

        var stockPrices = new[]
        {
            new { Ticker = "AAPL", Price = 150.00m, Timestamp = DateTime.UtcNow },
            new { Ticker = "GOOGL", Price = 2800.00m, Timestamp = DateTime.UtcNow },
            new { Ticker = "AMZN", Price = 3400.00m, Timestamp = DateTime.UtcNow },
            new { Ticker = "MSFT", Price = 4400.00m, Timestamp = DateTime.UtcNow }
        };

        foreach (var stock in stockPrices)
        {
            await connection.ExecuteAsync(
                "INSERT INTO public.stock_prices (ticker, price, timestamp) VALUES (@Ticker, @Price, @Timestamp)",
                stock
            );
        }
    }
}
