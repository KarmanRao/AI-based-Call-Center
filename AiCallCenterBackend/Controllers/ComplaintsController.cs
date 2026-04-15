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

            // 🔥 CATEGORY → DEPARTMENT
            complaint.Department = MapCategoryToDepartment(complaint.Category);

            // 🔥 FETCH SLA FROM DB
            var sla = await _context.SlaConfigs
                .FirstOrDefaultAsync(x => x.Category.ToLower() == complaint.Category.ToLower());

            if (sla == null)
                return BadRequest("No SLA defined for this category");

            // 🔥 APPLY SLA
            complaint.CurrentStageTime = TimeSpan.FromHours(sla.InitialTimeHours);
            complaint.ReductionTime = TimeSpan.FromHours(sla.ReductionHours);
            complaint.MinStageTime = TimeSpan.FromHours(sla.MinTimeHours);

            complaint.AssignedTo = "Technician";
            complaint.EscalationLevel = 0;
            complaint.AssignedAt = DateTime.UtcNow;

            complaint.StageDueAt = complaint.AssignedAt + complaint.CurrentStageTime;

            _context.Complaints.Add(complaint);
            await _context.SaveChangesAsync();

            await _smsQueue.EnqueueAsync(new SmsMessage(
                complaint.CallerPhone,
                $"Ticket {complaint.TicketId} registered"
            ));

            return Ok(complaint);
        }

        // ================= ✅ RESOLVE COMPLAINT =================
        [HttpPut("resolve/{id}")]
        public async Task<IActionResult> ResolveComplaint(int id)
        {
            var complaint = await _context.Complaints.FirstOrDefaultAsync(c => c.Id == id);

            if (complaint == null)
                return NotFound("Complaint not found");

            // 🔥 Update status
            complaint.Status = "Resolved";
            complaint.ResolvedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // 🔥 OPTIONAL: Send SMS on resolve
            await _smsQueue.EnqueueAsync(new SmsMessage(
                complaint.CallerPhone,
                $"Your complaint {complaint.TicketId} has been resolved."
            ));

            return Ok(complaint);
        }

        // ================= HELPER =================
        private static string MapCategoryToDepartment(string category)
        {
            var normalized = (category ?? "").ToLower();

            return normalized switch
            {
                "electricity" => "ElectricityDepartment",
                "water" => "WaterDepartment",
                "road" => "RoadDepartment",
                _ => "GeneralDepartment"
            };
        }
    }
}