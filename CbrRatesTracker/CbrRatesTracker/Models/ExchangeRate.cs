using Microsoft.EntityFrameworkCore;

namespace CbrRatesTracker.Models
{
    [Index(nameof(CurrencyId), nameof(Date), IsUnique = true)]
    public class ExchangeRate
    {
        public int Id { get; private set; }
        public int CurrencyId { get; private set; }
        public Currency Currency { get; private set; }
        public DateOnly Date { get; private set; }
        public int Nominal { get; private set; }
        public decimal Value { get; private set; }

        private ExchangeRate() { }

        public ExchangeRate(Currency currency, DateOnly date, int nominal, decimal value)
        {
            CurrencyId = currency.Id;
            Currency = currency;
            Date = date;
            Nominal = nominal;
            Value = value;
        }
    }
}
