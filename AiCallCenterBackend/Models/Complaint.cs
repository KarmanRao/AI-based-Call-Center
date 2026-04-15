namespace AiCallCenterBackend.Models
{
    public class Complaint
    {
        public int Id { get; set; }

        public string TicketId { get; set; } = string.Empty;

        // USER
        public string UserName { get; set; } = string.Empty;
        public string CallerPhone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;

        public int WardId { get; set; }
        public int AreaId { get; set; }

        // COMPLAINT
        public string Category { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
        public string Status { get; set; } = "New";

        // ESCALATION
        public string AssignedTo { get; set; } = "Technician";
        public int EscalationLevel { get; set; } = 0;
        public DateTime AssignedAt { get; set; }

        // 🔥 SLA SYSTEM
        public TimeSpan CurrentStageTime { get; set; }
        public TimeSpan ReductionTime { get; set; }
        public TimeSpan MinStageTime { get; set; }

        public DateTime StageDueAt { get; set; }

        // ✅ ✅ ADD THIS ONLY
        public DateTime? ResolvedAt { get; set; }

        // 🔴 FUTURE (Oracle)
        // TimeSpan may need conversion if Oracle has issues:
        // Option: store as NUMBER (minutes) instead of TimeSpan
    }
}