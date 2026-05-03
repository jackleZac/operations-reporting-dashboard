using Microsoft.EntityFrameworkCore;
using OperationsReportingDashboard.Models;

namespace OperationsReportingDashboard.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) 
            : base(options) { }

        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Car> Cars { get; set; }
        public DbSet<Maintenance> Maintenances { get; set; }
    }
}