using AiCallCenterBackend.Services;
using Microsoft.AspNetCore.Mvc;

namespace AiCallCenterBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController : ControllerBase
    {
        // Admin view: see all SMS logs
        [HttpGet]
        public IActionResult GetAll()
        {
            lock (NotificationStore.LockObj)
            {
                var result = NotificationStore.Items
                    .OrderByDescending(n => n.CreatedAtUtc)
                    .ToList();

                return Ok(result);
            }
        }

        // Admin view: see SMS for a ticket
        [HttpGet("ticket/{ticketId}")]
        public IActionResult GetByTicket(string ticketId)
        {
            lock (NotificationStore.LockObj)
            {
                var result = NotificationStore.Items
                    .Where(n => n.TicketId == ticketId)
                    .OrderByDescending(n => n.CreatedAtUtc)
                    .ToList();

                return Ok(result);
            }
        }

        // Admin view: see SMS for a phone number
        [HttpGet("to/{phone}")]
        public IActionResult GetByPhone(string phone)
        {
            lock (NotificationStore.LockObj)
            {
                var result = NotificationStore.Items
                    .Where(n => n.To == phone)
                    .OrderByDescending(n => n.CreatedAtUtc)
                    .ToList();

                return Ok(result);
            }
        }
    }
}
