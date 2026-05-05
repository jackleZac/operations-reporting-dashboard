using OperationsReportingDashboard.Data;
using Microsoft.EntityFrameworkCore;
using OperationsReportingDashboard.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=app.db"));
builder.Services.AddHttpClient();
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dashboard}/{action=Index}/{id?}")
    .WithStaticAssets();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (!db.Cars.Any())
    {
        var random = new Random();

        for (int i = 0; i < 15; i++)
        {
            db.Cars.Add(new Car
            {
                Make = "Toyota",
                Model = "Model " + i,
                PricePerDay = random.Next(100, 300),
                Transmission = "Auto",
                Seats = 5,
                Fuel = "Petrol",
                IsFeatured = random.Next(2) == 1,
                Status = "Available",
                CreatedAt = DateTime.Now
            });
        }

        db.SaveChanges();
    }

    if (!db.Bookings.Any())
    {
        var random = new Random();

        for (int i = 0; i < 200; i++)
        {
            db.Bookings.Add(new Booking
            {
                UserId = random.Next(1, 50),
                CarId = random.Next(1, 15),
                TotalPrice = random.Next(100, 500),
                StartDate = DateTime.Now.AddDays(random.Next(-30, 30)),
                EndDate = DateTime.Now.AddDays(random.Next(31, 60)),
                PickupLocation = "Street " + random.Next(1, 10),
                DropoffLocation = "Street " + random.Next(1, 10),
                Status = new[] { "Pending", "Confirmed", "Paid", "Active", "Completed", "Cancelled" }[random.Next(6)],
                CreatedAt = DateTime.Now.AddDays(-random.Next(0, 365))
            });
        }

        db.SaveChanges();
    }

    if (!db.Maintenances.Any())
    {
        var random = new Random();

        var serviceTypes = Enum.GetValues<ServiceType>();

        var statuses = new[]
        {
            "opened",
            "in progress",
            "completed"
        };

        var technicians = new[]
        {
            "Ahmad Rahman",
            "John Tan",
            "David Wong",
            "Zulkarnain Abdul",
            "Michael Lee"
        };

        var descriptions = new[]
        {
            "Routine maintenance check",
            "Customer reported issue after rental",
            "Scheduled service based on mileage",
            "Inspection before next rental",
            "Repair after long-distance usage",
            "Preventive maintenance work",
            "Minor issue detected during inspection"
        };

        var parts = new[]
        {
            "Engine oil, oil filter",
            "Brake pads",
            "Air filter",
            "Spark plugs",
            "Tires",
            "Battery",
            "Wiper blades",
            "Coolant",
            "Transmission fluid",
            null
        };

        for (int i = 0; i < 120; i++)
        {
            var serviceDate = DateTime.Now.AddDays(-random.Next(0, 365));
            
            db.Maintenances.Add(new Maintenance
            {
                CarId = random.Next(1, 16),
                UserId = random.Next(1, 50),

                LastRentalDate = serviceDate.AddDays(-random.Next(1, 20)),
                ServiceDate = serviceDate,

                ServiceType = serviceTypes[random.Next(serviceTypes.Length)],

                Description = descriptions[random.Next(descriptions.Length)],
                TechnicianName = technicians[random.Next(technicians.Length)],
                PartsReplaced = parts[random.Next(parts.Length)],

                TotalCost = random.Next(50, 2500),

                Status = statuses[random.Next(statuses.Length)]
            });
        }

        db.SaveChanges();
    }
}

app.Run();
