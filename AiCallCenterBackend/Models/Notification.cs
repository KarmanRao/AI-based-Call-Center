namespace AiCallCenterBackend.Models
{
    public class Notification
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        // For now it's "SMS". Later we can also add "WhatsApp", "Email", etc.
        public string Channel { get; set; } = "SMS";

        // Phone number
        public string To { get; set; } = string.Empty;

        // Ticket reference (so we can show SMS per complaint in dashboard)
        public string TicketId { get; set; } = string.Empty;

        // Actual message text
        public string Message { get; set; } = string.Empty;

        // When it was created (keep UTC for internal correctness)
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
