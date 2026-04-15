using AiCallCenterBackend.Models;
using AiCallCenterBackend.Services;
using AiCallCenterBackend.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AiCallCenterBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ComplaintsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly SmsQueue _smsQueue;

        public ComplaintsController(AppDbContext context, SmsQueue smsQueue)
        {
            _context = context;
            _smsQueue = smsQueue;
        }

        // ================= GET ALL =================
        [HttpGet]
        public async Task<ActionResult<List<Complaint>>> GetAll()
        {
            return await _context.Complaints
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        // ================= CREATE =================
        [HttpPost]
        public async Task<ActionResult<Complaint>> Create(Complaint complaint)
        {
            complaint.TicketId = $"TKT-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";
            complaint.CreatedAt = DateTime.UtcNow;
            complaint.Status = "New";

            complaint.AssignedTo = "Technician";
            complaint.EscalationLevel = 0;
            complaint.AssignedAt = DateTime.UtcNow;

            complaint.StageDueAt = complaint.AssignedAt + complaint.CurrentStageTime;

            _context.Complaints.Add(complaint);
            await _context.SaveChangesAsync();

            return Ok(complaint);
        }

        // ================= RESOLVE =================
        [HttpPut("resolve/{id}")]
        public async Task<IActionResult> ResolveComplaint(int id)
        {
            var complaint = await _context.Complaints.FirstOrDefaultAsync(c => c.Id == id);

            if (complaint == null)
                return NotFound();

            complaint.Status = "Resolved";
            complaint.ResolvedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(complaint);
        }

        // ================= ESCALATE (FINAL FIXED LOGIC) =================
        [HttpPut("escalate/{id}")]
        public async Task<IActionResult> EscalateComplaint(int id)
        {
            var complaint = await _context.Complaints.FirstOrDefaultAsync(c => c.Id == id);

            if (complaint == null)
                return NotFound("Complaint not found");

            // ❌ No escalation after resolve
            if (complaint.Status == "Resolved")
                return BadRequest("Cannot escalate resolved complaint");

            // ❌ Max escalation reached
            if (complaint.EscalationLevel >= 3)
                return BadRequest("Already at Director level. No further escalation allowed.");

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

            await _context.SaveChangesAsync();

            return Ok(complaint);
        }
    }
}