using AiCallCenterBackend.Models;

namespace AiCallCenterBackend.Services
{
    public class NotificationStore
    {
        public static readonly object LockObj = new();
        public static readonly List<Notification> Items = new();
    }
}
