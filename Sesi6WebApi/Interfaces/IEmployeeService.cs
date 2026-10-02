
using Sesi6WebApi.DTO.Common;
using Sesi6WebApi.DTO.Employee;

namespace Sesi6WebApi.Interfaces
{
    public interface IEmployeeService
    {
        Task<PagedResponse<EmployeeResponse>> GetAllAsync(EmployeeQuery query);
        Task<EmployeeResponse> GetByIdAsync(int id);
        Task<EmployeeResponse> CreateAsync(CreateEmployeeRequest request);
        Task UpdateAsync(int id, UpdateEmployeeRequest request);
        Task DeleteAsync(int id);
    }
}
