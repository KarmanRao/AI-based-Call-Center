namespace AiCallCenterBackend.Models
{
    public class Officer
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty; 
        // Example: Technician, Supervisor, Manager, Head

        public int Level { get; set; } 
        // 0 = Technician, 1 = Supervisor, 2 = Manager, etc.

        public string Department { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
    }
}