using AiCallCenterBackend.Models;
using AiCallCenterBackend.Services;
using AiCallCenterBackend.Data;
using Microsoft.AspNetCore.Mvc;

namespace AiCallCenterBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ComplaintsController : ControllerBase
    {
        private readonly SmsQueue _smsQueue;

        public ComplaintsController(SmsQueue smsQueue)
        {
            _smsQueue = smsQueue;
        }

        [HttpGet]
        public ActionResult<List<ComplaintDto>> GetAll()
        {
            lock (ComplaintStore.Lock)
            {
                var result = ComplaintStore.Complaints
                    .Select(ComplaintDto.FromComplaint)
                    .ToList();

                return Ok(result);
            }
        }

        [HttpPost]
        public async Task<ActionResult<Complaint>> Create(Complaint complaint)
        {
            lock (ComplaintStore.Lock)
            {
                complaint.TicketId = $"TKT-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";
                complaint.CreatedAt = DateTime.UtcNow;
                complaint.Status = "New";

                complaint.EscalationLevel = 0;
                complaint.AssignedTo = "Technician";
                complaint.AssignedAt = DateTime.UtcNow;

                ComplaintStore.Complaints.Add(complaint);
            }

            await _smsQueue.EnqueueAsync(new SmsMessage(
                complaint.CallerPhone,
                $"Ticket {complaint.TicketId} created"
            ));

            return Ok(complaint);
        }
    }
}