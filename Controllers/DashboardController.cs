using Microsoft.AspNetCore.Mvc;
using OperationsReportingDashboard.Data;

namespace OperationsReportingDashboard.Controllers
{
    public class DashboardController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;

        public DashboardController(AppDbContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            // Fetch all bookings from the database
            var bookings = _context.Bookings.ToList();

            // Fetch all cars from the database
            var cars = _context.Cars.ToList();

            // Calculate total revenue
            var totalRevenue = bookings.Sum(b => b.TotalPrice);

            // Calculate bookings this year
            var totalBookings = bookings.Count(b => b.CreatedAt.Year == DateTime.Now.Year);
            var active = bookings.Where(b => b.CreatedAt.Year == DateTime.Now.Year).Count(b => b.Status == "Active");
            var completed = bookings.Where(b => b.CreatedAt.Year == DateTime.Now.Year).Count(b => b.Status == "Completed");
            var cancelled = bookings.Where(b => b.CreatedAt.Year == DateTime.Now.Year).Count(b => b.Status == "Cancelled");

            // --- Monthly Revenue: This Month, Last 3 Months, Last 6 Months ---
            var monthlyRevenue6M = bookings
                .Where(b => b.CreatedAt >= DateTime.Now.AddMonths(-6))
                .GroupBy(b => new { b.CreatedAt.Year, b.CreatedAt.Month })
                .Select(g => new
                {
                    Month = g.Key.Month,
                    Year = g.Key.Year,
                    Revenue = g.Sum(x => x.TotalPrice)
                })
                .OrderBy(x => x.Year).ThenBy(x => x.Month)
                .ToList();

            var monthlyRevenue3M = bookings
                .Where(b => b.CreatedAt >= DateTime.Now.AddMonths(-3))
                .GroupBy(b => new { b.CreatedAt.Year, b.CreatedAt.Month })
                .Select(g => new
                {
                    Month = g.Key.Month,
                    Year = g.Key.Year,
                    Revenue = g.Sum(x => x.TotalPrice)
                })
                .OrderBy(x => x.Year).ThenBy(x => x.Month)
                .ToList();

            var monthlyRevenueThisMonth = bookings
                .Where(b => b.CreatedAt.Year == DateTime.Now.Year && b.CreatedAt.Month == DateTime.Now.Month)
                .GroupBy(b => new { b.CreatedAt.Year, b.CreatedAt.Month })
                .Select(g => new
                {
                    Month = g.Key.Month,
                    Year = g.Key.Year,
                    Revenue = g.Sum(x => x.TotalPrice)
                })
                .OrderBy(x => x.Year).ThenBy(x => x.Month)
                .ToList();

            // Get current month revenue
            var currentMonth = DateTime.Now.Month;
            var currentMonthRevenue = monthlyRevenue6M.LastOrDefault(m => m.Month == currentMonth)?.Revenue ?? 0;

            // Calculate revenue of the same month last year (for YoY comparison)
            var revenueSameMonthLastYear = bookings
                .Where(b => b.CreatedAt.Month == currentMonth && b.CreatedAt.Year == DateTime.Now.Year - 1)
                .Sum(b => b.TotalPrice);

            // Get revenue of the previous month (for MoM comparison)
            var previousMonth = DateTime.Now.AddMonths(-1).Month;
            var previousMonthRevenue = monthlyRevenue6M.LastOrDefault(m => m.Month == previousMonth)?.Revenue ?? 0;

            // --- Revenue Breakdown by Cars: This Month, Last 3 Months, Last 6 Months ---
            var revenueByCar6M = bookings
                .Where(b => b.CreatedAt >= DateTime.Now.AddMonths(-6))
                .GroupBy(b => b.CarId)
                .Select(g => new
                {
                    CarId = g.Key,
                    Revenue = g.Sum(x => x.TotalPrice)
                })
                .ToList();

            var revenueByCar3M = bookings
                .Where(b => b.CreatedAt >= DateTime.Now.AddMonths(-3))
                .GroupBy(b => b.CarId)
                .Select(g => new
                {
                    CarId = g.Key,
                    Revenue = g.Sum(x => x.TotalPrice)
                })
                .ToList();

            var revenueByCarThisMonth = bookings
                .Where(b => b.CreatedAt.Year == DateTime.Now.Year && b.CreatedAt.Month == DateTime.Now.Month)
                .GroupBy(b => b.CarId)
                .Select(g => new
                {
                    CarId = g.Key,
                    Revenue = g.Sum(x => x.TotalPrice)
                })
                .ToList();

            // Pass the calculated data to the view using ViewBag
            ViewBag.TotalRevenue = totalRevenue;
            ViewBag.TotalBookings = totalBookings;
            ViewBag.Active = active;
            ViewBag.Completed = completed;
            ViewBag.Cancelled = cancelled;

            ViewBag.MonthlyRevenue6M = monthlyRevenue6M;
            ViewBag.MonthlyRevenue3M = monthlyRevenue3M;
            ViewBag.MonthlyRevenueThisMonth = monthlyRevenueThisMonth;

            ViewBag.RevenueSameMonthLastYear = revenueSameMonthLastYear;
            ViewBag.PreviousMonthRevenue = previousMonthRevenue;
            ViewBag.CurrentMonthRevenue = currentMonthRevenue;

            ViewBag.RevenueByCar6M = revenueByCar6M;
            ViewBag.RevenueByCar3M = revenueByCar3M;
            ViewBag.RevenueByCarThisMonth = revenueByCarThisMonth;

            return View();
        }
    }
}