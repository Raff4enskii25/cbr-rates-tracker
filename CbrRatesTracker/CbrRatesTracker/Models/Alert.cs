using CbrRatesTracker.Enums;

namespace CbrRatesTracker.Models
{
    public class Alert
    {
        public int Id { get; private set; }
        public int CurrencyId { get; private set; }
        public Currency Currency { get; private set; }
        public decimal ThresholdValue { get; private set; }
        public Direction Direction { get; private set; }
        public bool IsTriggered { get; private set; }
        public DateOnly CreatedAt { get; private set; }
        public DateOnly? TriggeredAt { get; private set; }

        private Alert() { }

        public Alert(int currencyId, decimal thresholdValue, Direction direction)
        {
            CurrencyId = currencyId;
            ThresholdValue = thresholdValue;
            Direction = direction;
            IsTriggered = false;
            CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow);
            TriggeredAt = null;
        }
    }
}
