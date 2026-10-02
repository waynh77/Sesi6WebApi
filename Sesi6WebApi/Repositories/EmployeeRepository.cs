using Microsoft.EntityFrameworkCore;
using Sesi6WebApi.Data;
using Sesi6WebApi.DTO.Employee;
using Sesi6WebApi.Interfaces;
using Sesi6WebApi.Models;

namespace Sesi6WebApi.Repositories
{
    public sealed class EmployeeRepository(AppDbContext db) : IEmployeeRepository
    {
        public async Task<(List<Employee> Items, int TotalItems)> GetPagedAsync(
            EmployeeQuery query)
        {
            var q = db.Employees
                .AsNoTracking()
                .Include(x => x.Department)
                .Include(x => x.Position)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var search = query.Search.Trim();
                q = q.Where(x =>
                    x.FullName.Contains(search) ||
                    x.Email.Contains(search) ||
                    x.EmployeeNumber.Contains(search));
            }

            var total = await q.CountAsync();

            var items = await q
                .OrderBy(x => x.Id)
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();

            return (items, total);
        }

        public Task<Employee?> GetByIdAsync(int id) =>
            db.Employees
                .Include(x => x.Department)
                .Include(x => x.Position)
                .FirstOrDefaultAsync(x => x.Id == id);

        public Task<bool> ExistsByEmployeeNumberAsync(string employeeNumber) =>
            db.Employees.AnyAsync(x => x.EmployeeNumber == employeeNumber);

        public Task<bool> ExistsByEmailAsync(string email, int? excludeId) =>
            db.Employees.AnyAsync(x => x.Email == email && (!excludeId.HasValue || x.Id != excludeId.Value));

        public async Task AddAsync(Employee employee) =>
            await db.Employees.AddAsync(employee);

        public void Remove(Employee employee) => db.Employees.Remove(employee);

        public Task SaveChangesAsync() =>
            db.SaveChangesAsync();

        public Task<bool> DepartmentExistsAsync(int id) =>
            db.Departments.AnyAsync(x => x.Id == id);

        public Task<bool> PositionExistsAsync(int id) =>
            db.Positions.AnyAsync(x => x.Id == id);

    }
}
