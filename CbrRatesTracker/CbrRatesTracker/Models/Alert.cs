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
        public string? Email { get; private set; }

        private Alert() { }

        public Alert(Currency currency, decimal thresholdValue, Direction direction, string? email)
        {
            Currency = currency;
            CurrencyId = currency.Id;
            ThresholdValue = thresholdValue;
            Direction = direction;
            IsTriggered = false;
            CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow);
            TriggeredAt = null;
            
            if(email != null)
                Email = email;
        }

        public void SetAlertIsTriggered(DateOnly date)
        {
            IsTriggered = true;
            TriggeredAt = date;
        }
    }
}
