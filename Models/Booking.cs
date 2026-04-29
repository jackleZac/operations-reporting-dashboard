using System;

namespace OperationsReportingDashboard.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public required int UserId { get; set; }
        public required int CarId { get; set; }
        public Car Car { get; set; } = null!;
        public required decimal TotalPrice { get; set; }
        public required DateTime StartDate { get; set; }
        public required DateTime EndDate { get; set; }
        public required string PickupLocation { get; set; }
        public required string DropoffLocation { get; set; }
        public required string Status { get; set; } // pending, confirmed, paid, active, completed, cancelled
        public required DateTime CreatedAt { get; set; }
    }
}