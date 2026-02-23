using System.Threading.Channels;

namespace AiCallCenterBackend.Services
{
    public record SmsMessage(string ToPhone, string Text);

    public class SmsQueue
    {
        private readonly Channel<SmsMessage> _channel = Channel.CreateUnbounded<SmsMessage>();

        public ValueTask EnqueueAsync(SmsMessage message)
            => _channel.Writer.WriteAsync(message);

        public IAsyncEnumerable<SmsMessage> DequeueAllAsync(CancellationToken ct)
            => _channel.Reader.ReadAllAsync(ct);
    }
}
