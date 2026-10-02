using AutoMapper;
using Sesi6WebApi.DTO.Employee;
using Sesi6WebApi.Models;

namespace Sesi6WebApi.Mappings
{
    public sealed class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Employee, EmployeeResponse>()
                .ForMember(d => d.DepartmentName, o => o.MapFrom(s => s.Department!.Name))
                .ForMember(d => d.PositionName, o => o.MapFrom(s => s.Position!.Name));
            CreateMap<CreateEmployeeRequest, Employee>();
            CreateMap<UpdateEmployeeRequest, Employee>();
        }
    }
}
