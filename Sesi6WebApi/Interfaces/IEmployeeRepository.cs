using Sesi6WebApi.DTO.Employee;
using Sesi6WebApi.Models;

namespace Sesi6WebApi.Interfaces
{
        public interface IEmployeeRepository
        {
            Task<(List<Employee> Items, int TotalItems)> GetPagedAsync(EmployeeQuery query, CancellationToken cancellationToken);
            Task<Employee?> GetByIdAsync(int id, CancellationToken cancellationToken);
            Task<bool> ExistsByEmployeeNumberAsync(string employeeNumber, CancellationToken cancellationToken);
            Task<bool> ExistsByEmailAsync(string email, int? excludeId, CancellationToken cancellationToken);
            Task AddAsync(Employee employee, CancellationToken cancellationToken);
            void Remove(Employee employee);
            Task SaveChangesAsync(CancellationToken cancellationToken);
            Task<bool> DepartmentExistsAsync(int id, CancellationToken cancellationToken);
            Task<bool> PositionExistsAsync(int id, CancellationToken cancellationToken);
        }
}
