using AiCallCenterBackend.Controllers;
using Microsoft.Extensions.Hosting;

namespace AiCallCenterBackend.Services
{
    // Background worker: checks complaints and escalates when StageDueAt passes
    public class EscalationService : BackgroundService
    {
        private readonly SmsQueue _smsQueue;

        public EscalationService(SmsQueue smsQueue)
        {
            _smsQueue = smsQueue;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    lock (ComplaintsController.ComplaintsLock)
                    {
                        var nowUtc = DateTime.UtcNow;

                        foreach (var complaint in ComplaintsController.ComplaintsStore)
                        {
                            // Skip if resolved
                            if (string.Equals(complaint.Status, "Resolved", StringComparison.OrdinalIgnoreCase))
                                continue;

                            // Only 1 escalation for now (0 -> 1)
                            if (complaint.EscalationLevel >= 1)
                                continue;

                            // If not due yet, skip
                            if (nowUtc <= complaint.StageDueAt)
                                continue;

                            // ✅ Escalate to head
                            complaint.EscalationLevel = 1;
                            complaint.Status = "Escalated";

                            complaint.AssignedTo = ComplaintsController.GetDepartmentHead(complaint.Department);
                            complaint.AssignedAt = nowUtc;

                            var afterEscalation = ComplaintsController.GetAfterEscalationTimeForSeverity(complaint.Severity);
                            complaint.StageDueAt = complaint.AssignedAt.Add(afterEscalation);

                            complaint.EscalationNote =
                                $"Escalated to head at {complaint.AssignedAt:yyyy-MM-dd HH:mm:ss} UTC | " +
                                $"Severity={complaint.Severity} | New StageDueAt={complaint.StageDueAt:HH:mm:ss} UTC";

                            // ✅ DEMO SMS: escalation update
                            _ = _smsQueue.EnqueueAsync(new SmsMessage(
                                complaint.CallerPhone,
                                $"Ticket {complaint.TicketId}: Escalated. Now assigned to {complaint.AssignedTo}. Status={complaint.Status}"
                            ));
                        }
                    }
                }
                catch
                {
                    // Keep service alive even if something fails
                }

                // Check every 5 seconds (trial)
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }
}
