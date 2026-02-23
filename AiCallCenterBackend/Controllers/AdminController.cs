using AiCallCenterBackend.Models;
using Microsoft.AspNetCore.Mvc;

namespace AiCallCenterBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        [HttpGet("complaints")]
        public ActionResult<List<ComplaintDto>> GetAllComplaints()
        {
            lock (ComplaintsController.ComplaintsLock)
            {
                var result = ComplaintsController.ComplaintsStore
                    .OrderByDescending(c => c.CreatedAt)
                    .Select(ComplaintDto.FromComplaint)
                    .ToList();

                return Ok(result);
            }
        }
    }
}
