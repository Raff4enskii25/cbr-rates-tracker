using CbrRatesTracker.DTO;

namespace CbrRatesTracker.Services
{
    public record RateQueryResult
    {
        public bool IsSuccess { get; }
        public RateQueryError? Error { get; }
        public DateOnly From { get; }
        public DateOnly To { get; }
        public IEnumerable<ExchangeRateDTO> Rates { get; }

        private RateQueryResult(bool isSuccess, RateQueryError? error, DateOnly from, DateOnly to, IEnumerable<ExchangeRateDTO> rates)
        {
            IsSuccess = isSuccess;
            Error = error;
            From = from;
            To = to;
            Rates = rates;
        }

        public static RateQueryResult Success(DateOnly from, DateOnly to, IEnumerable<ExchangeRateDTO> rates)
        {
            return new RateQueryResult(true, null, from, to, rates);
        }

        public static RateQueryResult Fail(RateQueryError error)
        {
            return new RateQueryResult(false, error, default, default, Array.Empty<ExchangeRateDTO>());
        }
    }
}
