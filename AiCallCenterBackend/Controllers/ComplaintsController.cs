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
        public async Task<ActionResult<List<ComplaintDto>>> GetAll()
        {
            var complaints = await _context.Complaints
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            var result = complaints
                .Select(ComplaintDto.FromComplaint)
                .ToList();

            return Ok(result);
        }

        // ================= GET BY TICKET =================
        [HttpGet("{ticketId}")]
        public async Task<ActionResult<ComplaintDto>> GetByTicketId(string ticketId)
        {
            var complaint = await _context.Complaints
                .FirstOrDefaultAsync(c => c.TicketId == ticketId);

            if (complaint == null)
                return NotFound("Complaint not found");

            return Ok(ComplaintDto.FromComplaint(complaint));
        }

        // ================= CREATE =================
        [HttpPost]
        public async Task<ActionResult<ComplaintDto>> Create(Complaint complaint)
        {
            // 🔥 Auto-generated fields
            complaint.TicketId = $"TKT-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";
            complaint.CreatedAt = DateTime.UtcNow;
            complaint.Status = "New";

            // 🔥 Map Category → Department
            complaint.Department = MapCategoryToDepartment(complaint.Category);

            // 🔥 Initial assignment
            complaint.EscalationLevel = 0;
            complaint.AssignedTo = "Technician";
            complaint.AssignedAt = DateTime.UtcNow;

            _context.Complaints.Add(complaint);
            await _context.SaveChangesAsync();

            // 🔥 Simulated SMS
            await _smsQueue.EnqueueAsync(new SmsMessage(
                complaint.CallerPhone,
                $"Ticket {complaint.TicketId}: Registered in {complaint.Department}"
            ));

            return Ok(ComplaintDto.FromComplaint(complaint));
        }

        // ================= UPDATE STATUS =================
        [HttpPatch("{ticketId}/status")]
        public async Task<ActionResult<ComplaintDto>> UpdateStatus(string ticketId, UpdateStatusRequest request)
        {
            var complaint = await _context.Complaints
                .FirstOrDefaultAsync(c => c.TicketId == ticketId);

            if (complaint == null)
                return NotFound("Complaint not found");

            if (string.IsNullOrWhiteSpace(request.Status))
                return BadRequest("Status is required");

            complaint.Status = request.Status.Trim();

            await _context.SaveChangesAsync();

            return Ok(ComplaintDto.FromComplaint(complaint));
        }

        // ================= RESOLVE =================
        [HttpPatch("{ticketId}/resolve")]
        public async Task<ActionResult<ComplaintDto>> Resolve(string ticketId, ResolveRequest request)
        {
            var complaint = await _context.Complaints
                .FirstOrDefaultAsync(c => c.TicketId == ticketId);

            if (complaint == null)
                return NotFound("Complaint not found");

            complaint.Status = "Resolved";

            await _context.SaveChangesAsync();

            return Ok(ComplaintDto.FromComplaint(complaint));
        }

        // ================= HELPER =================
        private static string MapCategoryToDepartment(string category)
        {
            var normalized = (category ?? "").Trim().ToLower();

            return normalized switch
            {
                "water" => "WaterDepartment",
                "road" => "RoadDepartment",
                "electricity" => "ElectricityDepartment",
                _ => "GeneralDepartment"
            };
        }
    }

    // ================= DTOs =================

    public class UpdateStatusRequest
    {
        public string Status { get; set; } = string.Empty;
    }

    public class ResolveRequest
    {
        public string? Note { get; set; }
    }
}