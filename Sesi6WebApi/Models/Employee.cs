
using System.ComponentModel.DataAnnotations;

namespace Sesi6WebApi.Models
{
    public class Employee
    {
        public int Id { get; set; }
        [Required]
        public string EmployeeNumber { get; set; } = string.Empty;
        [Required]
        public string FullName { get; set; } = string.Empty;
        [Required]
        public string Email { get; set; } = string.Empty;
        public decimal Salary { get; set; }
        public int DepartmentId { get; set; }
        public int PositionId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Department? Department { get; set; }
        public Position? Position { get; set; }
    }

}
