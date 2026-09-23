namespace CbrRatesTracker.DTO
{
    public record ExchangeRateDTO(
        DateOnly Date,
        int Nominal,
        decimal Value,
        CurrencyDTO Currency
    );
}
