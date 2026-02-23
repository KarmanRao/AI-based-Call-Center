namespace AiCallCenterBackend.Models
{
    public class Complaint
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string TicketId { get; set; } = string.Empty;

        // -------- User Details (for dashboard) --------
        public string UserName { get; set; } = string.Empty;
        public string CallerPhone { get; set; } = string.Empty; // contact number
        public string Address { get; set; } = string.Empty;
        public string WardNo { get; set; } = string.Empty;

        // -------- Complaint Details --------
        public string Category { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;

        public string Summary { get; set; } = string.Empty;
        public string FullConversation { get; set; } = string.Empty;

        // Registration date/time (this is "date when complaint is registered")
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string Status { get; set; } = "New";

        // -------- Assignment / Escalation --------
        public string AssignedTo { get; set; } = string.Empty;
        public int EscalationLevel { get; set; } = 0; // 0=Technician, 1=Head (for now)
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
        public string EscalationNote { get; set; } = string.Empty;

        // -------- Severity + SLA (trial) --------
        public string Severity { get; set; } = "Medium"; // Low/Medium/High
        public DateTime SlaDueAt { get; set; }
        public DateTime StageDueAt { get; set; }

        // -------- Resolution (dashboard needs it) --------
        public string ResolutionNote { get; set; } = string.Empty;
        public string ResolvedBy { get; set; } = string.Empty;
        public DateTime? ResolvedAt { get; set; }

        public DateTime CreatedAtIst => CreatedAt.AddHours(5).AddMinutes(30);
        public DateTime AssignedAtIst => AssignedAt.AddHours(5).AddMinutes(30);
        public DateTime StageDueAtIst => StageDueAt.AddHours(5).AddMinutes(30);
        public DateTime SlaDueAtIst => SlaDueAt.AddHours(5).AddMinutes(30);
        public DateTime? ResolvedAtIst => ResolvedAt?.AddHours(5).AddMinutes(30);

    }
}
