using Microsoft.EntityFrameworkCore;

namespace CbrRatesTracker.Models
{
    [Index(nameof(CharCode), IsUnique = true)]
    public class Currency
    {
        public int Id { get; private set; }
        public string CharCode { get; private set; }
        public int NumCode { get; private set; }
        public string Name { get; private set; }

        private Currency() { }

        public Currency(string charCode, int numCode, string name)
        {
            CharCode = charCode;
            NumCode = numCode;
            Name = name;
        }
    }
}
