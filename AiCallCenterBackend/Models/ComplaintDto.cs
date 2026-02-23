using AiCallCenterBackend.Models;

namespace AiCallCenterBackend.Models
{
    public class ComplaintDto
    {
        public Guid Id { get; set; }
        public string TicketId { get; set; } = string.Empty;

        public string UserName { get; set; } = string.Empty;
        public string CallerPhone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string WardNo { get; set; } = string.Empty;

        public string Category { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;

        public string Summary { get; set; } = string.Empty;
        public string FullConversation { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }     // IST in response
        public string Status { get; set; } = string.Empty;

        public string AssignedTo { get; set; } = string.Empty;
        public int EscalationLevel { get; set; }
        public DateTime AssignedAt { get; set; }    // IST in response
        public string EscalationNote { get; set; } = string.Empty;

        public string Severity { get; set; } = string.Empty;
        public DateTime SlaDueAt { get; set; }      // IST in response
        public DateTime StageDueAt { get; set; }    // IST in response

        public string ResolutionNote { get; set; } = string.Empty;
        public string ResolvedBy { get; set; } = string.Empty;
        public DateTime? ResolvedAt { get; set; }   // IST in response

        // ✅ Mapper: Complaint (UTC stored) -> ComplaintDto (IST returned)
        public static ComplaintDto FromComplaint(Complaint c)
        {
            return new ComplaintDto
            {
                Id = c.Id,
                TicketId = c.TicketId,

                UserName = c.UserName,
                CallerPhone = c.CallerPhone,
                Address = c.Address,
                WardNo = c.WardNo,

                Category = c.Category,
                Department = c.Department,

                Summary = c.Summary,
                FullConversation = c.FullConversation,

                CreatedAt = ToIst(c.CreatedAt),
                Status = c.Status,

                AssignedTo = c.AssignedTo,
                EscalationLevel = c.EscalationLevel,
                AssignedAt = ToIst(c.AssignedAt),
                EscalationNote = c.EscalationNote,

                Severity = c.Severity,
                SlaDueAt = ToIst(c.SlaDueAt),
                StageDueAt = ToIst(c.StageDueAt),

                ResolutionNote = c.ResolutionNote,
                ResolvedBy = c.ResolvedBy,
                ResolvedAt = c.ResolvedAt.HasValue ? ToIst(c.ResolvedAt.Value) : null
            };
        }

        private static DateTime ToIst(DateTime utc)
        {
            return utc.AddHours(5).AddMinutes(30);
        }
    }
}
