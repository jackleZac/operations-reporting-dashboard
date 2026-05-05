using Microsoft.AspNetCore.Mvc;
using OperationsReportingDashboard.Data;
using OperationsReportingDashboard.Models;
using Microsoft.EntityFrameworkCore;

namespace OperationsReportingDashboard.Controllers
{
    public class FleetMaintenanceController : Controller
    {
        private readonly AppDbContext _context;

        public FleetMaintenanceController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var now = DateTime.Now;
            var currentMonth = now.Month;
            var currentYear = now.Year;

            var maintenanceRecords = _context.Maintenances.ToList();

            // Calculate number of maintenance works this month by status
            var thisMonthMaintenance = maintenanceRecords
                .Where(m => m.ServiceDate.HasValue &&
                            m.ServiceDate.Value.Month == currentMonth &&
                            m.ServiceDate.Value.Year == currentYear)
                .ToList();

            // Total expense per ServiceType
            var expensePerServiceType = maintenanceRecords
                .Where(m => m.TotalCost.HasValue
                    && m.ServiceDate.HasValue 
                    && m.ServiceDate.Value.Month == currentMonth)
                .GroupBy(m => m.ServiceType)
                .Select(g => new
                {
                    serviceType = g.Key.ToString(),
                    totalExpense = g.Sum(m => m.TotalCost ?? 0)
                })
                .OrderByDescending(x => x.totalExpense)
                .ToList();


            // Calculate number of services per ServiceType
            var serviceCountPerType = maintenanceRecords
                .Where(m => m.ServiceDate.HasValue 
                    && m.ServiceDate.Value.Year == currentYear)
                .GroupBy(m => m.ServiceType)
                .Select(g => new
                {
                    serviceType = g.Key.ToString(),
                    count = g.Count()
                })
                .OrderByDescending(x => x.count)
                .ToList();


            // Calculate total maintenance cost every month for last 6 months
            var sixMonthsAgo = now.AddMonths(-5);

            var monthlyMaintenanceCost = maintenanceRecords
                .Where(m => m.ServiceDate.HasValue &&
                            m.ServiceDate.Value >= new DateTime(sixMonthsAgo.Year, sixMonthsAgo.Month, 1))
                .GroupBy(m => new
                {
                    Year = m.ServiceDate!.Value.Year,
                    Month = m.ServiceDate!.Value.Month
                })
                .Select(g => new
                {
                    year = g.Key.Year,
                    month = g.Key.Month,
                    totalCost = g.Sum(m => m.TotalCost ?? 0)
                })
                .OrderBy(x => x.year)
                .ThenBy(x => x.month)
                .ToList();


            // Identify high-maintenance cars (this year)
            // Criteria: Cars whose maintenance costs exceed a threshold (20%) relative to their rental revenue
            var highMaintenanceCars = _context.Cars
                .Include(c => c.Bookings)
                .Select(car => new
                {
                    carId = car.Id,
                    carName = car.Make + " " + car.Model,

                    totalRentalRevenue = car.Bookings
                        .Where(b => b.StartDate.Year == currentYear)
                        .Sum(b => b.TotalPrice),

                    totalMaintenanceCost = _context.Maintenances
                        .Where(
                            m => m.CarId == car.Id &&
                            m.ServiceDate.HasValue &&
                            m.ServiceDate.Value.Year == currentYear)
                        .Sum(m => m.TotalCost ?? 0)
                })
                .AsEnumerable()
                .Where(c => c.totalRentalRevenue > 0 &&
                            c.totalMaintenanceCost >= c.totalRentalRevenue * 0.20m)
                .Select(c => new
                {
                    carId = c.carId,
                    carName = c.carName,
                    totalRentalRevenue = c.totalRentalRevenue,
                    totalMaintenanceCost = c.totalMaintenanceCost,
                    maintenanceRatio = Math.Round((c.totalMaintenanceCost / c.totalRentalRevenue) * 100, 2)
                })
                .OrderByDescending(c => c.maintenanceRatio)
                .Take(5)
                .ToList();

            ViewBag.OpenedCount = thisMonthMaintenance
                .Count(m => m.Status.ToLower() == "opened");
            ViewBag.InProgressCount = thisMonthMaintenance
                .Count(m => m.Status.ToLower() == "in progress");
            ViewBag.CompletedCount = thisMonthMaintenance
                .Count(m => m.Status.ToLower() == "completed");
            ViewBag.ExpensePerServiceType = expensePerServiceType;
            ViewBag.ServiceCountPerType = serviceCountPerType;
            ViewBag.MonthlyMaintenanceCost = monthlyMaintenanceCost;
            ViewBag.HighMaintenanceCars = highMaintenanceCars;

            return View();
        }
    }
}