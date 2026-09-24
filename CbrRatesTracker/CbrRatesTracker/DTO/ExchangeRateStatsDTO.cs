namespace CbrRatesTracker.DTO
{
    public record ExchangeRateStatsDTO(DateOnly From, DateOnly To, decimal MaxRate, decimal MinRate, decimal AverageRate);
}
