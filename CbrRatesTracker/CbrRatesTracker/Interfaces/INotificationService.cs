namespace CbrRatesTracker.Interfaces
{
    public interface INotificationService
    {
        Task SendMessageAsync(string recipient, string subject, string body);
    }
}
