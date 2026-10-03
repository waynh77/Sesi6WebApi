
using Sesi6WebApi.DTO.Common;
using Sesi6WebApi.DTO.Employee;

namespace Sesi6WebApi.Interfaces
{
    public interface IEmployeeService
    {
        Task<PagedResponse<EmployeeResponse>> GetAllAsync(EmployeeQuery query, CancellationToken cancellationToken);
        Task<EmployeeResponse> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<EmployeeResponse> CreateAsync(CreateEmployeeRequest request, CancellationToken cancellationToken);
        Task UpdateAsync(int id, UpdateEmployeeRequest request, CancellationToken cancellationToken);
        Task DeleteAsync(int id, CancellationToken cancellationToken);
    }
}
