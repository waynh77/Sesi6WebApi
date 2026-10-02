using Sesi6WebApi.DTO.Employee;
using Sesi6WebApi.Models;

namespace Sesi6WebApi.Interfaces
{
    public interface IEmployeeRepository
    {
        Task<(List<Employee> Items, int TotalItems)> GetPagedAsync(EmployeeQuery query);
        Task<Employee?> GetByIdAsync(int id);
        Task<bool> ExistsByEmployeeNumberAsync(string employeeNumber);
        Task<bool> ExistsByEmailAsync(string email, int? excludeId);
        Task AddAsync(Employee employee);
        void Remove(Employee employee);
        Task SaveChangesAsync();
        Task<bool> DepartmentExistsAsync(int id);
        Task<bool> PositionExistsAsync(int id);
    }
}
