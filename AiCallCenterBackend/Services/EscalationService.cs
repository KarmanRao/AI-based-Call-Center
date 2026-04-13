using Microsoft.EntityFrameworkCore;
using AiCallCenterBackend.Data;
using AiCallCenterBackend.Models;

namespace AiCallCenterBackend.Services
{
    public class EscalationService
    {
        private readonly AppDbContext _context;

        public EscalationService(AppDbContext context)
        {
            _context = context;
        }

        public async Task AutoEscalateComplaints()
        {
            var complaints = await _context.Complaints
                .Where(c => c.Status != "Resolved")
                .ToListAsync();

            foreach (var complaint in complaints)
            {
                if (complaint.EscalationLevel >= 4)
                    continue;

                if (DateTime.UtcNow >= complaint.StageDueAt)
                {
                    complaint.EscalationLevel++;

                    complaint.AssignedTo = EscalationHelper.GetNextLevel(complaint.EscalationLevel);

                    // 🔥 REDUCE TIME
                    complaint.CurrentStageTime -= complaint.ReductionTime;

                    if (complaint.CurrentStageTime < complaint.MinStageTime)
                    {
                        complaint.CurrentStageTime = complaint.MinStageTime;
                    }

                    complaint.AssignedAt = DateTime.UtcNow;
                    complaint.StageDueAt = complaint.AssignedAt + complaint.CurrentStageTime;
                }
            }

            await _context.SaveChangesAsync();

            // 🔴 FUTURE (Oracle)
            // This SaveChanges will persist data in real DB automatically
        }
    }
}