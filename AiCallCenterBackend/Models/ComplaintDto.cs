namespace AiCallCenterBackend.Models
{
    public class ComplaintDto
    {
        public int Id { get; set; }
        public string TicketId { get; set; } = string.Empty;

        // User
        public string UserName { get; set; } = string.Empty;
        public string CallerPhone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;

        public int WardId { get; set; }
        public int AreaId { get; set; }

        // Complaint
        public string Category { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        // Escalation
        public string AssignedTo { get; set; } = string.Empty;
        public int EscalationLevel { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime AssignedAt { get; set; }

        public static ComplaintDto FromComplaint(Complaint c)
        {
            return new ComplaintDto
            {
                Id = c.Id,
                TicketId = c.TicketId,
                UserName = c.UserName,
                CallerPhone = c.CallerPhone,
                Address = c.Address,
                WardId = c.WardId,
                AreaId = c.AreaId,
                Category = c.Category,
                Description = c.Description,
                Department = c.Department,
                Status = c.Status,
                AssignedTo = c.AssignedTo,
                EscalationLevel = c.EscalationLevel,
                CreatedAt = c.CreatedAt,
                AssignedAt = c.AssignedAt
            };
        }
    }
}