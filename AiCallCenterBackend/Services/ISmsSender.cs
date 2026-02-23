namespace AiCallCenterBackend.Services
{
    public interface ISmsSender
    {
        Task SendAsync(string toPhone, string text, CancellationToken ct = default);
    }
}
