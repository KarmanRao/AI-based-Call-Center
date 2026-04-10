using AiCallCenterBackend.Models;

namespace AiCallCenterBackend.Data
{
    public static class ComplaintStore
    {
        public static readonly object Lock = new();
        public static readonly List<Complaint> Complaints = new();
    }
}