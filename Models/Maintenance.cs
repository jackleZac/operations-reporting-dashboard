using System;

namespace OperationsReportingDashboard.Models
{
    public class Maintenance
    {
        public int Id { get; set; }
        public required int CarId { get; set; }
        public int? UserId { get; set; }
        public DateTime? LastRentalDate { get; set; } // to link usage with wear/tear
        public DateTime? ServiceDate { get; set; }
        public ServiceType ServiceType { get; set; }
        public string? Description { get; set; }
        public string? TechnicianName { get; set; }
        public string? PartsReplaced { get; set; }
        public decimal? TotalCost { get; set; }
        public required string Status { get; set; } // opened, in progress, completed
    }
}