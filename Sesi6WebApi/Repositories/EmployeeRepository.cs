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
            EmployeeQuery query, CancellationToken cancellationToken)
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

            var total = await q.CountAsync(cancellationToken);

            var items = await q
                .OrderBy(x => x.Id)
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync(cancellationToken);

            return (items, total);
        }

        public Task<Employee?> GetByIdAsync(int id, CancellationToken cancellationToken) =>
            db.Employees
                .Include(x => x.Department)
                .Include(x => x.Position)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        public Task<bool> ExistsByEmployeeNumberAsync(string employeeNumber, CancellationToken cancellationToken) =>
            db.Employees.AnyAsync(x => x.EmployeeNumber == employeeNumber, cancellationToken);

        public Task<bool> ExistsByEmailAsync(string email, int? excludeId, CancellationToken cancellationToken) =>
            db.Employees.AnyAsync(x => x.Email == email && (!excludeId.HasValue || x.Id != excludeId.Value), cancellationToken);

        public async Task AddAsync(Employee employee, CancellationToken cancellationToken) =>
            await db.Employees.AddAsync(employee, cancellationToken);

        public void Remove(Employee employee) => db.Employees.Remove(employee);

        public Task SaveChangesAsync(CancellationToken cancellationToken) =>
            db.SaveChangesAsync(cancellationToken);

        public Task<bool> DepartmentExistsAsync(int id, CancellationToken cancellationToken) =>
            db.Departments.AnyAsync(x => x.Id == id, cancellationToken);

        public Task<bool> PositionExistsAsync(int id, CancellationToken cancellationToken) =>
            db.Positions.AnyAsync(x => x.Id == id, cancellationToken);
    }
}
