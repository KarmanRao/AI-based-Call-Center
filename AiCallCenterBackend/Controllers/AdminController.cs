using AiCallCenterBackend.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AiCallCenterBackend.Controllers
{
    [ApiController]
    [Route("api/admin")]
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            var complaints = await _context.Complaints.ToListAsync();

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