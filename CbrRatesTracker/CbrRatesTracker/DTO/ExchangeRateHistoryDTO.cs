namespace CbrRatesTracker.DTO
{
    public record ExchangeRateHistoryDTO(string Name, DateOnly From, DateOnly To, IEnumerable<ExchangeRateDTO> Rate);
}
