
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
        public async Task<PagedResponse<EmployeeResponse>> GetAllAsync(EmployeeQuery query)
        {
            var result = await repository.GetPagedAsync(query);
            return new PagedResponse<EmployeeResponse>
            {
                Items = mapper.Map<List<EmployeeResponse>>(result.Items),
                Page = query.Page,
                PageSize = query.PageSize,
                TotalItems = result.TotalItems
            };
        }

        public async Task<EmployeeResponse> GetByIdAsync(int id)
        {
            var employee = await repository.GetByIdAsync(id)
                ?? throw new NotFoundException($"Employee dengan id {id} tidak ditemukan.");

            return mapper.Map<EmployeeResponse>(employee);
        }

        public async Task<EmployeeResponse> CreateAsync(CreateEmployeeRequest request)
        {
            if (await repository.ExistsByEmployeeNumberAsync(request.EmployeeNumber))
                throw new BusinessException("EmployeeNumber sudah digunakan.");

            if (await repository.ExistsByEmailAsync(request.Email, null))
                throw new BusinessException("Email employee sudah digunakan.");

            if (!await repository.DepartmentExistsAsync(request.DepartmentId))
                throw new BusinessException("Department tidak ditemukan.");

            if (!await repository.PositionExistsAsync(request.PositionId))
                throw new BusinessException("Position tidak ditemukan.");

            var employee = mapper.Map<Employee>(request);
            employee.CreatedAt = DateTime.UtcNow;

            await repository.AddAsync(employee);
            await repository.SaveChangesAsync();

            var saved = await repository.GetByIdAsync(employee.Id)
                ?? throw new InvalidOperationException("Employee gagal dibaca setelah disimpan.");

            return mapper.Map<EmployeeResponse>(saved);
        }

        public async Task UpdateAsync(int id, UpdateEmployeeRequest request)
        {
            var employee = await repository.GetByIdAsync(id)
                ?? throw new NotFoundException($"Employee dengan id {id} tidak ditemukan.");

            if (await repository.ExistsByEmailAsync(request.Email, id))
                throw new BusinessException("Email employee sudah digunakan.");

            if (!await repository.DepartmentExistsAsync(request.DepartmentId))
                throw new BusinessException("Department tidak ditemukan.");

            if (!await repository.PositionExistsAsync(request.PositionId))
                throw new BusinessException("Position tidak ditemukan.");

            mapper.Map(request, employee);
            await repository.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var employee = await repository.GetByIdAsync(id)
                ?? throw new NotFoundException($"Employee dengan id {id} tidak ditemukan.");

            repository.Remove(employee);
            await repository.SaveChangesAsync();
        }
    }
}
