using Microsoft.EntityFrameworkCore;

namespace CbrRatesTracker.Data
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
    }
}
