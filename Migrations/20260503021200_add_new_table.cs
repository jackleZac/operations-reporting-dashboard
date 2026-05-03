using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OperationsReportingDashboard.Migrations
{
    /// <inheritdoc />
    public partial class add_new_table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Maintenances",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CarId = table.Column<int>(type: "INTEGER", nullable: false),
                    UserId = table.Column<int>(type: "INTEGER", nullable: true),
                    LastRentalDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ServiceDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ServiceType = table.Column<int>(type: "INTEGER", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    TechnicianName = table.Column<string>(type: "TEXT", nullable: true),
                    PartsReplaced = table.Column<string>(type: "TEXT", nullable: true),
                    TotalCost = table.Column<decimal>(type: "TEXT", nullable: true),
                    Status = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Maintenances", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Maintenances");
        }
    }
}
