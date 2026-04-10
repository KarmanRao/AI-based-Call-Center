using AiCallCenterBackend.Data;
using Microsoft.AspNetCore.Mvc;

namespace AiCallCenterBackend.Controllers
{
    [ApiController]
    [Route("api/admin")]
    public class AdminController : ControllerBase
    {
        [HttpGet("stats")]
        public IActionResult GetStats()
        {
            lock (ComplaintStore.Lock)
            {
                var complaints = ComplaintStore.Complaints;

                var total = complaints.Count;
                var resolved = complaints.Count(c => c.Status == "Resolved");
                var pending = complaints.Count(c => c.Status != "Resolved");

                return Ok(new
                {
                    Total = total,
                    Resolved = resolved,
                    Pending = pending
                });
            }
        }
    }
}