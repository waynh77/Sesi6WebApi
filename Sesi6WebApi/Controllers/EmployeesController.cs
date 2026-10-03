using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sesi6WebApi.DTO.Common;
using Sesi6WebApi.DTO.Employee;
using Sesi6WebApi.Interfaces;

namespace Sesi6WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public sealed class EmployeesController(IEmployeeService service, ILogger<EmployeesController> logger)
    : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<PagedResponse<EmployeeResponse>>> GetAll(
            [FromQuery] EmployeeQuery query, CancellationToken cancellationToken)
            => Ok(await service.GetAllAsync(query, cancellationToken));

        [HttpGet("{id:int}")]
        public async Task<ActionResult<EmployeeResponse>> GetById(
            int id, CancellationToken cancellationToken)
            => Ok(await service.GetByIdAsync(id, cancellationToken));

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<EmployeeResponse>> Create(
            CreateEmployeeRequest request, CancellationToken cancellationToken)
        {
            var result = await service.CreateAsync(request, cancellationToken);
            logger.LogInformation("Employee {EmployeeId} created by {User}", result.Id, User.Identity?.Name);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        //[Authorize(Roles = "Admin")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
            int id, UpdateEmployeeRequest request, CancellationToken cancellationToken)
        {
            await service.UpdateAsync(id, request, cancellationToken);
            logger.LogInformation("Employee {EmployeeId} updated by {User}", id, User.Identity?.Name);
            return NoContent();
        }

        //[Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            await service.DeleteAsync(id, cancellationToken);
            logger.LogInformation("Employee {EmployeeId} deleted by {User}", id, User.Identity?.Name);
            return NoContent();
        }
    }
}
