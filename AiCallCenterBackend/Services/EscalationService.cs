using AiCallCenterBackend.Models;
using AiCallCenterBackend.Data;
using Microsoft.EntityFrameworkCore;

namespace AiCallCenterBackend.Services
{
    public class EscalationService
    {
        private readonly AppDbContext _context;

        public EscalationService(AppDbContext context)
        {
            _context = context;
        }

        // ================= MANUAL ESCALATION =================
        public void ApplyEscalation(Complaint complaint)
        {
            if (complaint.Status == "Resolved")
                return;

            if (complaint.EscalationLevel >= 3)
                return;

            complaint.EscalationLevel += 1;
            complaint.AssignedAt = DateTime.UtcNow;
            complaint.StageDueAt = complaint.AssignedAt + complaint.CurrentStageTime;

            complaint.AssignedTo = complaint.EscalationLevel switch
            {
                1 => "Junior Officer",
                2 => "Senior Officer",
                3 => "Director",
                _ => "Director"
            };
        }

        // ================= AUTO ESCALATION (FIX FOR YOUR ERROR) =================
        public async Task AutoEscalateComplaints()
        {
            var now = DateTime.UtcNow;

            var complaints = await _context.Complaints
                .Where(c => c.Status != "Resolved" &&
                            c.EscalationLevel < 3 &&
                            c.StageDueAt <= now)
                .ToListAsync();

            foreach (var complaint in complaints)
            {
                ApplyEscalation(complaint);
            }

            await _context.SaveChangesAsync();
        }
    }
}