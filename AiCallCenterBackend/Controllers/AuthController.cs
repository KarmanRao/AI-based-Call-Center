using Microsoft.AspNetCore.Mvc;

namespace AiCallCenterBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private static List<User> users = new List<User>
        {
            new User 
            { 
                Username = "admin", 
                Password = "admin123", 
                Role = "Admin", 
                Department = "" 
            },

            // 🔥 SAMPLE TECHNICIANS (AS PER REAL DEPARTMENTS)

            new User 
            { 
                Username = "tech_road", 
                Password = "123", 
                Role = "Technician", 
                Department = "Road"
            },

            new User 
            { 
                Username = "tech_water", 
                Password = "123", 
                Role = "Technician", 
                Department = "Water Works"
            },

            new User 
            { 
                Username = "tech_drainage", 
                Password = "123", 
                Role = "Technician", 
                Department = "Drainage Projects"
            },

            new User 
            { 
                Username = "tech_light", 
                Password = "123", 
                Role = "Technician", 
                Department = "Street Light"
            }
        };

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var user = users.FirstOrDefault(u =>
                u.Username == request.Username &&
                u.Password == request.Password);

            if (user == null)
                return Unauthorized("Invalid credentials");

            return Ok(new
            {
                username = user.Username,
                role = user.Role,
                department = user.Department
            });
        }
    }

    public class LoginRequest
    {
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
    }

    public class User
    {
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
        public string Role { get; set; } = "";
        public string Department { get; set; } = "";
    }
}