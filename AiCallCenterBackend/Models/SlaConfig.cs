namespace AiCallCenterBackend.Models
{
    public class SlaConfig
    {
        public int Id { get; set; }

        public string Category { get; set; } = string.Empty;

        public double InitialTimeHours { get; set; }
        public double ReductionHours { get; set; }
        public double MinTimeHours { get; set; }

        // 🔴 FUTURE
        // This table will be manually managed from DB (Admin Panel later)
    }
}