using AiCallCenterBackend.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AiCallCenterBackend.Services
{
    public class SmsWorker : BackgroundService
    {
        private readonly SmsQueue _queue;
        private readonly ISmsSender _smsSender;
        private readonly ILogger<SmsWorker> _logger;

        public SmsWorker(SmsQueue queue, ISmsSender smsSender, ILogger<SmsWorker> logger)
        {
            _queue = queue;
            _smsSender = smsSender;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await foreach (var msg in _queue.DequeueAllAsync(stoppingToken))
            {
                try
                {
                    // 1) "Send" SMS (demo sender just logs)
                    await _smsSender.SendAsync(msg.ToPhone, msg.Text, stoppingToken);

                    // 2) Store it in NotificationStore (acts like DB for now)
                    lock (NotificationStore.LockObj)
                    {
                        NotificationStore.Items.Add(new Notification
                        {
                            Channel = "SMS",
                            To = msg.ToPhone,
                            TicketId = ExtractTicketId(msg.Text),
                            Message = msg.Text,
                            CreatedAtUtc = DateTime.UtcNow
                        });
                    }

                    _logger.LogInformation("SMS stored in NotificationStore for {To}", msg.ToPhone);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "SMS failed for {To}", msg.ToPhone);
                }
            }
        }

        private static string ExtractTicketId(string text)
        {
            // Supports:
            // "Ticket TKT-ABC12345: ..."  OR  "Ticket TKT-ABC12345 ..."
            if (string.IsNullOrWhiteSpace(text)) return "";

            var marker = "Ticket ";
            var idx = text.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
            if (idx < 0) return "";

            var start = idx + marker.Length;

            // TicketId ends at ':' OR space OR end of string
            var colonPos = text.IndexOf(':', start);
            var spacePos = text.IndexOf(' ', start);

            int end;
            if (colonPos >= 0 && spacePos >= 0) end = Math.Min(colonPos, spacePos);
            else if (colonPos >= 0) end = colonPos;
            else if (spacePos >= 0) end = spacePos;
            else end = text.Length;

            if (end <= start) return "";

            return text.Substring(start, end - start).Trim();
        }
    }
}
