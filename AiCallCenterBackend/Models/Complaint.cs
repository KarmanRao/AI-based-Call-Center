namespace AiCallCenterBackend.Models
{
    public class Complaint
    {
        public int Id { get; set; }

        public string TicketId { get; set; } = string.Empty;

        public string UserName { get; set; } = string.Empty;
        public string CallerPhone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;

        public int WardId { get; set; }
        public int AreaId { get; set; }

        public string Category { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
        public string Status { get; set; } = "New";

        public string AssignedTo { get; set; } = "Technician";
        public int EscalationLevel { get; set; } = 0;
        public DateTime AssignedAt { get; set; }

        public TimeSpan CurrentStageTime { get; set; }
        public TimeSpan ReductionTime { get; set; }
        public TimeSpan MinStageTime { get; set; }

        public DateTime StageDueAt { get; set; }

        // ================= RESOLUTION =================
        public string? ResolutionNote { get; set; }
        public DateTime? ResolvedAt { get; set; }

        // 📸 IMAGE PROOF (BASE64)
        public string? ResolutionImageBase64 { get; set; }
    }
}