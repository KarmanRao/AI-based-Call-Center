namespace AiCallCenterBackend.Models
{
    public class SlaConfig
    {
        public int Id { get; set; }

        public string Category { get; set; } = string.Empty;

        public int InitialTimeHours { get; set; }   // e.g. 12
        public int ReductionHours { get; set; }     // e.g. 2
        public int MinTimeHours { get; set; }       // e.g. 4
    }
}