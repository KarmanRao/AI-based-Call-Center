using AiCallCenterBackend.Models;
using Microsoft.Extensions.Logging;

namespace AiCallCenterBackend.Services
{
    public class FakeSmsSender : ISmsSender
    {
        private readonly ILogger<FakeSmsSender> _logger;

        public FakeSmsSender(ILogger<FakeSmsSender> logger)
        {
            _logger = logger;
        }

        public Task SendAsync(string toPhone, string text, CancellationToken ct = default)
        {
            // SmsQueue passes only phone+text.
            // TicketId will be embedded in text (we do that when enqueueing).
            _logger.LogInformation("DEMO SMS -> {To}: {Text}", toPhone, text);
            return Task.CompletedTask;
        }
    }
}
