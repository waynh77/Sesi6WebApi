
namespace Sesi6WebApi.DTO.Employee
{
    public sealed class EmployeeQuery
    {
        public string? Search { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

}
