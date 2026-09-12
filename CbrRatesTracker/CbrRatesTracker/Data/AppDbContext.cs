using Microsoft.EntityFrameworkCore;
using CbrRatesTracker.Models;

namespace CbrRatesTracker.Data
{
    public class AppDbContext: DbContext
    {
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<ExchangeRate> ExchangeRates { get; set; }
        public DbSet<Alert> Alerts { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
    }
}
