
using AutoMapper;
using Sesi6WebApi.DTO.Common;
using Sesi6WebApi.DTO.Employee;
using Sesi6WebApi.Exceptions;
using Sesi6WebApi.Interfaces;
using Sesi6WebApi.Models;

namespace Sesi6WebApi.Services
{
    public sealed class EmployeeService(IEmployeeRepository repository, IMapper mapper) : IEmployeeService
    {
        public async Task<PagedResponse<EmployeeResponse>> GetAllAsync(EmployeeQuery query, CancellationToken cancellationToken)
        {
            var result = await repository.GetPagedAsync(query, cancellationToken);
            return new PagedResponse<EmployeeResponse>
            {
                Items = mapper.Map<List<EmployeeResponse>>(result.Items),
                Page = query.Page,
                PageSize = query.PageSize,
                TotalItems = result.TotalItems
            };
        }

        public async Task<EmployeeResponse> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            var employee = await repository.GetByIdAsync(id, cancellationToken)
                ?? throw new NotFoundException($"Employee dengan id {id} tidak ditemukan.");

            return mapper.Map<EmployeeResponse>(employee);
        }

        public async Task<EmployeeResponse> CreateAsync(CreateEmployeeRequest request, CancellationToken cancellationToken)
        {
            if (await repository.ExistsByEmployeeNumberAsync(request.EmployeeNumber, cancellationToken))
                throw new BusinessException("EmployeeNumber sudah digunakan.");

            if (await repository.ExistsByEmailAsync(request.Email, null, cancellationToken))
                throw new BusinessException("Email employee sudah digunakan.");

            if (!await repository.DepartmentExistsAsync(request.DepartmentId, cancellationToken))
                throw new BusinessException("Department tidak ditemukan.");

            if (!await repository.PositionExistsAsync(request.PositionId, cancellationToken))
                throw new BusinessException("Position tidak ditemukan.");

            var employee = mapper.Map<Employee>(request);
            employee.CreatedAt = DateTime.UtcNow;

            await repository.AddAsync(employee, cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);

            var saved = await repository.GetByIdAsync(employee.Id, cancellationToken)
                ?? throw new InvalidOperationException("Employee gagal dibaca setelah disimpan.");

            return mapper.Map<EmployeeResponse>(saved);
        }

        public async Task UpdateAsync(int id, UpdateEmployeeRequest request, CancellationToken cancellationToken)
        {
            var employee = await repository.GetByIdAsync(id, cancellationToken)
                ?? throw new NotFoundException($"Employee dengan id {id} tidak ditemukan.");

            if (await repository.ExistsByEmailAsync(request.Email, id, cancellationToken))
                throw new BusinessException("Email employee sudah digunakan.");

            if (!await repository.DepartmentExistsAsync(request.DepartmentId, cancellationToken))
                throw new BusinessException("Department tidak ditemukan.");

            if (!await repository.PositionExistsAsync(request.PositionId, cancellationToken))
                throw new BusinessException("Position tidak ditemukan.");

            mapper.Map(request, employee);
            await repository.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken)
        {
            var employee = await repository.GetByIdAsync(id, cancellationToken)
                ?? throw new NotFoundException($"Employee dengan id {id} tidak ditemukan.");

            repository.Remove(employee);
            await repository.SaveChangesAsync(cancellationToken);
        }
    }
}
