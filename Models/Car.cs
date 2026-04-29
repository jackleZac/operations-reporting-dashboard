using System;

namespace OperationsReportingDashboard.Models
{
    public class Car
    {
        public int Id { get; set; }
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public required string Make { get; set; }
        public required string Model { get; set; }
        public required decimal PricePerDay { get; set; }
        public required string Transmission { get; set; }
        public required int Seats { get; set; }
        public required string Fuel { get; set; }
        public required bool IsFeatured { get; set; } 
        public required string Status { get; set; } // available, rented, maintenance
        public required DateTime CreatedAt { get; set; }
    }
}