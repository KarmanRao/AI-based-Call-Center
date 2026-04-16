using AiCallCenterBackend.Models;
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

        public ComplaintsController(AppDbContext context)
        {
            _context = context;
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

        // ================= ESCALATE =================
        [HttpPut("escalate/{id}")]
        public async Task<IActionResult> Escalate(int id)
        {
            var complaint = await _context.Complaints.FirstOrDefaultAsync(c => c.Id == id);

            if (complaint == null)
                return NotFound();

            if (complaint.Status == "Resolved")
                return BadRequest("Cannot escalate resolved complaint");

            if (complaint.EscalationLevel >= 3)
                return BadRequest("Already at max level");

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

        // ================= RESOLVE (UPDATED) =================
        [HttpPut("resolve/{id}")]
        public async Task<IActionResult> Resolve(int id, [FromBody] ResolveRequest request)
        {
            var complaint = await _context.Complaints.FirstOrDefaultAsync(c => c.Id == id);

            if (complaint == null)
                return NotFound();

            if (string.IsNullOrWhiteSpace(request.Note))
                return BadRequest("Resolution note required");

            if (string.IsNullOrWhiteSpace(request.ImageBase64))
                return BadRequest("Resolution image required");

            complaint.Status = "Resolved";
            complaint.ResolvedAt = DateTime.UtcNow;

            complaint.ResolutionNote = request.Note;
            complaint.ResolutionImageBase64 = request.ImageBase64;

            // ✅ NEW FIELD
            complaint.ResolvedBy = request.ResolvedBy;

            await _context.SaveChangesAsync();

            return Ok(complaint);
        }
    }

    // ================= DTO =================
    public class ResolveRequest
    {
        public string Note { get; set; } = "";
        public string ImageBase64 { get; set; } = "";

        // ✅ NEW
        public string ResolvedBy { get; set; } = "";
    }
}