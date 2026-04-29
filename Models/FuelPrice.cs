using System;
using System.Text.Json.Serialization;

namespace OperationsReportingDashboard.Models
{
    public class FuelPrice
    {   
        [JsonPropertyName("ron95")]
        public decimal? Ron95 { get; set; }

        [JsonPropertyName("ron97")]
        public decimal? Ron97 { get; set; }

        [JsonPropertyName("diesel_eastmsia")]
        public decimal? DieselEastMalaysia { get; set; }
        
        [JsonPropertyName("date")]
        public DateTime Date { get; set; }

        [JsonPropertyName("series_type")]
        public string? SeriesType { get; set; }
    }
}