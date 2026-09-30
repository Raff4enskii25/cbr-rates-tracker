using CbrRatesTracker.Enums;

namespace CbrRatesTracker.DTO
{
    public record AlertDTO(
        string CharCode,
        decimal ThresholdValue,
        Direction Direction,
        bool IsTriggered,
        DateOnly CreatedAt,
        DateOnly? TriggeredAt
    );
}
