using Microsoft.AspNetCore.Mvc;
using Sesi6WebApi.DTO.Common;
using Sesi6WebApi.DTO.Employee;
using Sesi6WebApi.Interfaces;

namespace Sesi6WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public sealed class EmployeesController(IEmployeeService service, ILogger<EmployeesController> logger)
    : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<PagedResponse<EmployeeResponse>>> GetAll(
            [FromQuery] EmployeeQuery query)
            => Ok(await service.GetAllAsync(query));

        [HttpGet("{id:int}")]
        public async Task<ActionResult<EmployeeResponse>> GetById(
            int id, CancellationToken cancellationToken)
            => Ok(await service.GetByIdAsync(id));

        [HttpPost]
        public async Task<ActionResult<EmployeeResponse>> Create(
            CreateEmployeeRequest request)
        {
            var result = await service.CreateAsync(request);
            logger.LogInformation("Employee {EmployeeId} created by {User}", result.Id, User.Identity?.Name);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
            int id, UpdateEmployeeRequest request)
        {
            await service.UpdateAsync(id, request);
            logger.LogInformation("Employee {EmployeeId} updated by {User}", id, User.Identity?.Name);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await service.DeleteAsync(id);
            logger.LogInformation("Employee {EmployeeId} deleted by {User}", id, User.Identity?.Name);
            return NoContent();
        }
    }
}
