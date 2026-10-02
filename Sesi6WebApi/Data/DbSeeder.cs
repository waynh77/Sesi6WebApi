using Microsoft.EntityFrameworkCore;
using Sesi6WebApi.Models;

namespace Sesi6WebApi.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(AppDbContext db)
        {
            try
            {
                await db.Database.MigrateAsync();
            }
            catch (Microsoft.Data.SqlClient.SqlException ex) when (ex.Number == 2714)
            {
                // Table already exists — migration may have been applied manually. Continue seeding.
            }

            if (!await db.Departments.AnyAsync())
            {
                db.Departments.AddRange(
                    new Department { Name = "IT" },
                    new Department { Name = "Finance" },
                    new Department { Name = "Human Resources" });
                await db.SaveChangesAsync();
            }

            if (!await db.Positions.AnyAsync())
            {
                db.Positions.AddRange(
                    new Position { Name = "Software Developer" },
                    new Position { Name = "Senior Developer" },
                    new Position { Name = "Manager" });
                await db.SaveChangesAsync();
            }


            if (!await db.Employees.AnyAsync())
            {
                var it = await db.Departments.SingleAsync(x => x.Name == "IT");
                var dev = await db.Positions.SingleAsync(x => x.Name == "Software Developer");

                db.Employees.AddRange(
                    new Employee
                    {
                        EmployeeNumber = "EMP001",
                        FullName = "Budi Santoso",
                        Email = "budi@example.com",
                        Salary = 10000000,
                        DepartmentId = it.Id,
                        PositionId = dev.Id
                    },
                    new Employee
                    {
                        EmployeeNumber = "EMP002",
                        FullName = "Siti Aminah",
                        Email = "siti@example.com",
                        Salary = 12000000,
                        DepartmentId = it.Id,
                        PositionId = dev.Id
                    });
                await db.SaveChangesAsync();
            }
        }
    }
}
