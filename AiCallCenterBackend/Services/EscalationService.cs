using AiCallCenterBackend.Data;

namespace AiCallCenterBackend.Services
{
    public class EscalationService
    {
        private static readonly List<string> EscalationLevels = new()
        {
            "Technician",
            "Supervisor",
            "Manager",
            "Director",
            "Head"
        };

        public Task AutoEscalateComplaints()
        {
            lock (ComplaintStore.Lock)
            {
                foreach (var complaint in ComplaintStore.Complaints)
                {
                    if (complaint.Status == "Resolved")
                        continue;

                    if (complaint.EscalationLevel >= EscalationLevels.Count - 1)
                        continue;

                    var timePassed = DateTime.UtcNow - complaint.AssignedAt;

                    if (timePassed.TotalMinutes >= 1)
                    {
                        complaint.EscalationLevel++;

                        complaint.AssignedTo = EscalationLevels[complaint.EscalationLevel];
                        complaint.AssignedAt = DateTime.UtcNow;

                        complaint.EscalationNote =
                            $"Escalated to {complaint.AssignedTo} at {complaint.AssignedAt}";
                    }
                }
            }

            return Task.CompletedTask;
        }
    }
}