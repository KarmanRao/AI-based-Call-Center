namespace AiCallCenterBackend.Services
{
    public static class EscalationHelper
    {
        public static string GetNextLevel(int level)
        {
            return level switch
            {
                1 => "Supervisor",
                2 => "Manager",
                3 => "Director",
                4 => "Head",
                _ => "Technician"
            };
        }
    }
}