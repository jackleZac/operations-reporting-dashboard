using Microsoft.AspNetCore.Mvc;
using OperationsReportingDashboard.Data;
using System;
using System.Linq;
using System.Text.Json;
using OperationsReportingDashboard.Models;

namespace OperationsReportingDashboard.Controllers
{
    public class DashboardController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;

        public DashboardController(AppDbContext context,  IHttpClientFactory httpClientFactory)
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

            // Calculate revenue by month for the last 6 months
            var monthlyRevenue = bookings
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

            // Get current month revenue
            var currentMonth = DateTime.Now.Month;
            var currentMonthRevenue = monthlyRevenue.LastOrDefault(m => m.Month == currentMonth)?.Revenue ?? 0;

            // Calculate revenue of the same month (for YoY comparison)of the previous year
            var revenueSameMonthLastYear = bookings
                .Where(b => b.CreatedAt.Month == currentMonth && b.CreatedAt.Year == DateTime.Now.Year - 1)
                .Sum(b => b.TotalPrice);

            // Get revenue of the previous month (for MoM comparison)
            var previousMonth = DateTime.Now.AddMonths(-1).Month;
            var previousMonthRevenue = monthlyRevenue.LastOrDefault(m => m.Month == previousMonth)?.Revenue ?? 0;
            
            // Total revenue by car model (current month)
            var monthlyRevenueByCar = bookings
                .Where(b => b.CreatedAt >= DateTime.Now.AddMonths(-1))
                .GroupBy(b => b.CarId)
                .Select(g => new
                {
                    CarId = g.Key,
                    Revenue = g.Sum(x => x.TotalPrice)
                })
                .ToList();

            // Fetch fuel price
            var client = _httpClientFactory.CreateClient();
            var json = await client.GetStringAsync("https://api.data.gov.my/data-catalogue?id=fuelprice");

            var fuelPrices = JsonSerializer.Deserialize<List<FuelPrice>>(json);
            
            // Latest fuel price
            var latestFuelPrice = fuelPrices?
                .Where(f => f.SeriesType == "level")
                .OrderByDescending(f => f.Date)
                .FirstOrDefault();

            // Fuel prices for the last 6 months
            var lastSixMonthsFuelPrices = fuelPrices ?
                .Where(f => f.Date >= DateTime.Now.AddMonths(-6) && f.SeriesType == "level")
                .OrderBy(f => f.Date)
                .ToList();

            // Pass the calculated data to the view using ViewBag
            ViewBag.TotalRevenue = totalRevenue;
            ViewBag.TotalBookings = totalBookings;
            ViewBag.Active = active;
            ViewBag.Completed = completed;
            ViewBag.Cancelled = cancelled;
            ViewBag.MonthlyRevenue = monthlyRevenue;
            ViewBag.RevenueSameMonthLastYear = revenueSameMonthLastYear;
            ViewBag.PreviousMonthRevenue = previousMonthRevenue;
            ViewBag.CurrentMonthRevenue = currentMonthRevenue;
            ViewBag.MonthlyRevenueByCar = monthlyRevenueByCar;
            ViewBag.LatestFuelPrices = latestFuelPrice;
            ViewBag.Ron97 = lastSixMonthsFuelPrices?.Select(f => f.Ron97).ToList();
            ViewBag.Ron95 = lastSixMonthsFuelPrices?.Select(f => f.Ron95).ToList();
            ViewBag.Diesel = lastSixMonthsFuelPrices?.Select(f => f.DieselEastMalaysia).ToList();
            ViewBag.FuelDates = lastSixMonthsFuelPrices?.Select(f => f.Date.ToString("dd MMM")).ToList();
            
            return View();
        }
    }
}