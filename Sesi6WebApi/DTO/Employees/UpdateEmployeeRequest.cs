
namespace Sesi6WebApi.DTO.Employee
{
    public sealed class UpdateEmployeeRequest
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public decimal Salary { get; set; }
        public int DepartmentId { get; set; }
        public int PositionId { get; set; }
    }
}
