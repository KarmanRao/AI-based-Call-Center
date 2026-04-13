using Microsoft.EntityFrameworkCore;
using AiCallCenterBackend.Data;

namespace AiCallCenterBackend.Services
{
    public class EscalationService
    {
        private readonly AppDbContext _context;

        private static readonly List<string> Levels = new()
        {
            "Technician",
            "Supervisor",
            "Manager",
            "Director",
            "Head"
        };

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
                if (complaint.EscalationLevel >= Levels.Count - 1)
                    continue;

                var timePassed = DateTime.UtcNow - complaint.AssignedAt;

                if (timePassed.TotalSeconds >= 10) // 🔥 change later as needed
                {
                    complaint.EscalationLevel++;
                    complaint.AssignedTo = Levels[complaint.EscalationLevel];
                    complaint.AssignedAt = DateTime.UtcNow;
                }
            }

            await _context.SaveChangesAsync(); // 🔥 DB SAVE POINT
        }
    }
}