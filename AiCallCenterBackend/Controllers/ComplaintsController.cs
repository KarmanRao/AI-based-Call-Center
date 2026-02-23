using AiCallCenterBackend.Models;
using AiCallCenterBackend.Services;
using Microsoft.AspNetCore.Mvc;

namespace AiCallCenterBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ComplaintsController : ControllerBase
    {
        // Temporary in-memory store (will be DB later)
        public static readonly object ComplaintsLock = new();
        public static readonly List<Complaint> ComplaintsStore = new();

        // SMS Queue (DI)
        private readonly SmsQueue _smsQueue;

        public ComplaintsController(SmsQueue smsQueue)
        {
            _smsQueue = smsQueue;
        }

        // ---------------- GET ALL (IST in Swagger via DTO) ----------------
        [HttpGet]
        public ActionResult<List<ComplaintDto>> GetAll()
        {
            lock (ComplaintsLock)
            {
                var result = ComplaintsStore
                    .OrderByDescending(c => c.CreatedAt)
                    .Select(ComplaintDto.FromComplaint)
                    .ToList();

                return Ok(result);
            }
        }

        // ---------------- GET BY TICKET ID (IST in Swagger via DTO) ----------------
        [HttpGet("{ticketId}")]
        public ActionResult<ComplaintDto> GetByTicketId(string ticketId)
        {
            lock (ComplaintsLock)
            {
                var complaint = ComplaintsStore.FirstOrDefault(c => c.TicketId == ticketId);
                if (complaint == null)
                    return NotFound("Complaint not found");

                return Ok(ComplaintDto.FromComplaint(complaint));
            }
        }

        // ---------------- CREATE COMPLAINT ----------------
        [HttpPost]
        public ActionResult<Complaint> Create(Complaint complaint)
        {
            lock (ComplaintsLock)
            {
                complaint.TicketId = $"TKT-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";
                complaint.CreatedAt = DateTime.UtcNow;
                complaint.Status = "New";

                // Map category → department
                complaint.Department = MapCategoryToDepartment(complaint.Category);

                // Map department → severity (trial)
                complaint.Severity = MapDepartmentToSeverity(complaint.Department);

                // Assign to technician (level 0)
                complaint.EscalationLevel = 0;
                complaint.AssignedTo = GetDepartmentTechnician(complaint.Department);
                complaint.AssignedAt = DateTime.UtcNow;

                // SLA calculations (trial times)
                complaint.SlaDueAt = complaint.CreatedAt.Add(GetTotalSlaForSeverity(complaint.Severity));
                complaint.StageDueAt = complaint.AssignedAt.Add(GetStage1TimeForSeverity(complaint.Severity));

                complaint.EscalationNote =
                    $"Assigned to technician at {complaint.AssignedAt:yyyy-MM-dd HH:mm:ss} UTC | " +
                    $"Severity={complaint.Severity} | StageDueAt={complaint.StageDueAt:HH:mm:ss} UTC";

                ComplaintsStore.Add(complaint);

                // ✅ DEMO SMS: complaint registered
                _ = _smsQueue.EnqueueAsync(new SmsMessage(
                    complaint.CallerPhone,
                    $"Ticket {complaint.TicketId}: Registered. Department={complaint.Department}. Status={complaint.Status}"
                ));

                return Ok(complaint);
            }
        }

        // ---------------- UPDATE STATUS ----------------
        [HttpPatch("{ticketId}/status")]
        public ActionResult<Complaint> UpdateStatus(string ticketId, UpdateStatusRequest request)
        {
            lock (ComplaintsLock)
            {
                var complaint = ComplaintsStore.FirstOrDefault(c => c.TicketId == ticketId);
                if (complaint == null)
                    return NotFound("Complaint not found");

                if (request == null || string.IsNullOrWhiteSpace(request.Status))
                    return BadRequest("Status is required");

                complaint.Status = request.Status.Trim();

                // ✅ DEMO SMS: status update
                _ = _smsQueue.EnqueueAsync(new SmsMessage(
                    complaint.CallerPhone,
                    $"Ticket {complaint.TicketId}: Status changed to {complaint.Status}"
                ));

                return Ok(complaint);
            }
        }

        // ---------------- RESOLVE COMPLAINT ----------------
        // Only current assignee can resolve
        [HttpPatch("{ticketId}/resolve")]
        public ActionResult<Complaint> Resolve(string ticketId, ResolveComplaintRequest request)
        {
            lock (ComplaintsLock)
            {
                var complaint = ComplaintsStore.FirstOrDefault(c => c.TicketId == ticketId);
                if (complaint == null)
                    return NotFound("Complaint not found");

                if (request == null || string.IsNullOrWhiteSpace(request.ResolvedBy))
                    return BadRequest("ResolvedBy is required");

                if (!string.Equals(request.ResolvedBy.Trim(), complaint.AssignedTo, StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest($"Not allowed. Only '{complaint.AssignedTo}' can resolve this complaint.");
                }

                complaint.Status = "Resolved";
                complaint.ResolvedBy = request.ResolvedBy.Trim();
                complaint.ResolvedAt = DateTime.UtcNow;
                complaint.ResolutionNote = request.ResolutionNote?.Trim() ?? "";

                complaint.EscalationNote =
                    $"Resolved by {complaint.ResolvedBy} at {complaint.ResolvedAt:yyyy-MM-dd HH:mm:ss} UTC";

                // ✅ DEMO SMS: resolved update
                _ = _smsQueue.EnqueueAsync(new SmsMessage(
                    complaint.CallerPhone,
                    $"Ticket {complaint.TicketId}: Resolved. Note={complaint.ResolutionNote}"
                ));

                return Ok(complaint);
            }
        }

        // ================= HELPER METHODS =================

        private static string MapCategoryToDepartment(string category)
        {
            var normalized = (category ?? "").Trim().ToLower();
            return normalized switch
            {
                "water" => "WaterSupplyDepartment",
                "road" => "RoadMaintenanceDepartment",
                "electricity" => "ElectricityDepartment",
                _ => "GeneralDepartment"
            };
        }

        private static string MapDepartmentToSeverity(string department)
        {
            return department switch
            {
                "WaterSupplyDepartment" => "Low",
                "RoadMaintenanceDepartment" => "Medium",
                "ElectricityDepartment" => "High",
                _ => "Medium"
            };
        }

        private static string GetDepartmentTechnician(string department)
        {
            return department switch
            {
                "WaterSupplyDepartment" => "WaterTech",
                "RoadMaintenanceDepartment" => "RoadTech",
                "ElectricityDepartment" => "ElectricityTech",
                _ => "GeneralTech"
            };
        }

        public static string GetDepartmentHead(string department)
        {
            return department switch
            {
                "WaterSupplyDepartment" => "WaterHead",
                "RoadMaintenanceDepartment" => "RoadHead",
                "ElectricityDepartment" => "ElectricityHead",
                _ => "GeneralHead"
            };
        }

        public static TimeSpan GetTotalSlaForSeverity(string severity)
        {
            return severity switch
            {
                "Low" => TimeSpan.FromMinutes(3),
                "Medium" => TimeSpan.FromMinutes(2),
                "High" => TimeSpan.FromMinutes(1),
                _ => TimeSpan.FromMinutes(2)
            };
        }

        public static TimeSpan GetStage1TimeForSeverity(string severity)
        {
            return severity switch
            {
                "Low" => TimeSpan.FromMinutes(3),
                "Medium" => TimeSpan.FromMinutes(2),
                "High" => TimeSpan.FromMinutes(1),
                _ => TimeSpan.FromMinutes(2)
            };
        }

        public static TimeSpan GetAfterEscalationTimeForSeverity(string severity)
        {
            return severity switch
            {
                "Low" => TimeSpan.FromMinutes(2),
                "Medium" => TimeSpan.FromMinutes(1),
                "High" => TimeSpan.FromSeconds(30),
                _ => TimeSpan.FromMinutes(1)
            };
        }
    }

    // ---------------- DTOs ----------------

    public class UpdateStatusRequest
    {
        public string Status { get; set; } = string.Empty;
    }

    public class ResolveComplaintRequest
    {
        public string ResolvedBy { get; set; } = string.Empty;
        public string? ResolutionNote { get; set; }
    }
}
