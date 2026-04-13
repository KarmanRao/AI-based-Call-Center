using Microsoft.EntityFrameworkCore;
using AiCallCenterBackend.Models;

namespace AiCallCenterBackend.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Complaint> Complaints { get; set; }
        public DbSet<SlaConfig> SlaConfigs { get; set; }

        // 🔴 FUTURE (Oracle)
        // When DB is connected:
        // 1. Tables will be created in Oracle
        // 2. Remove any in-memory seeding from Program.cs
    }
}