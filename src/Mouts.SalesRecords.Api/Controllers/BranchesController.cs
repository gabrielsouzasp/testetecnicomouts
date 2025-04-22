using Microsoft.AspNetCore.Mvc;
using Mouts.SalesRecords.Application.Core.Services.Interfaces;
using Mouts.SalesRecords.Domain.Entities;
using Mouts.SalesRecords.Domain.Requests;

namespace Mouts.SalesRecords.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BranchesController : ControllerBase
    {
        private readonly IBranchService _branchService;

        public BranchesController(IBranchService branchService)
        {
            _branchService = branchService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateBranchRequest request, CancellationToken cancellationToken = default)
        {
            await _branchService.CreateAsync(request, cancellationToken);
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> ReadAllAsync(CancellationToken cancellationToken = default)
        {
            var response = await _branchService.ReadAllAsync(cancellationToken);

            if (response.Count == 0) return NotFound();

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ReadAsync([FromRoute] Guid id, CancellationToken cancellationToken = default)
        {
            var response = await _branchService.ReadAsync(id, cancellationToken);

            if (response is null) return NotFound();

            return Ok(response);
        }


        [HttpPut]
        public async Task<IActionResult> PutAsync([FromBody] Branch request, CancellationToken cancellationToken = default)
        {
            var response = await _branchService.UpdateAsync(request, cancellationToken);

            if (!response) return NotFound(response);

            return Ok(response);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync([FromRoute] Guid id, CancellationToken cancellationToken = default)
        {
            var response = await _branchService.DeleteAsync(id, cancellationToken);

            if (!response) return NotFound(response);
            return Ok(response);
        }
    }
}
