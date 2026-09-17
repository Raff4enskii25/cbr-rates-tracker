namespace CbrRatesTracker.CbrIntegration
{
    public record ValuteDTO(
        string CharCode,
        int NumCode,
        int Nominal,
        string Name,
        decimal Value,
        DateOnly Date
    );
}
