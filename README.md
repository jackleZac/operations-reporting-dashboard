# Operations Reporting Dashboard
Built with ASP.NET Core MVC, Entity Framework Core and Chart.js, the system assists in managing assets and operations of a company, which in this case, is a car rental company. 

- Operational Performance
    - Monthly revenue
    - Revenue breakdown
    - Revenue comparison (YoY, MoM)
- Fleet Maintenance
    - Number of Maintenance Works
    - Total Cost of Maintenance 
    - Total Expense per Service Type
    - Number of Services per Service Type
    - High Maintenance Cars


## Prerequisite
- ASP.NET Core MVC,
- ChartJS

## User Interface
![Dashboard screenshot](screenshots/dashboard.jpeg)
![Maintenance screenshot](screenshots/maintenance.jpeg)

# Dotnet EF Commands
```
dotnet ef migrations add {{ describe the changes }}
dotnet ef database update
dotnet ef migrations remove
```