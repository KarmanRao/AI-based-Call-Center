using Microsoft.EntityFrameworkCore;
using AiCallCenterBackend.Models;

namespace AiCallCenterBackend.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Complaint> Complaints { get; set; }

        // 🟡 FUTURE (Oracle specific mapping)
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // modelBuilder.Entity<Complaint>().ToTable("COMPLAINTS"); // Enable for Oracle

            base.OnModelCreating(modelBuilder);
        }
    }
}