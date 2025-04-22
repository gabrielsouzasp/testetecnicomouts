using Microsoft.AspNetCore.Mvc;
using Mouts.SalesRecords.Application.Core.Services.Interfaces;
using Mouts.SalesRecords.Domain.Entities;
using Mouts.SalesRecords.Domain.Requests;

namespace Mouts.SalesRecords.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SalesController : ControllerBase
    {
        private readonly ISalesService _salesService;

        public SalesController(ISalesService salesService)
        {
            _salesService = salesService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateSaleRequest request, CancellationToken cancellationToken = default)
        {
            await _salesService.CreateAsync(request, cancellationToken);
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> ReadAllAsync(CancellationToken cancellationToken = default)
        {
            var response = await _salesService.ReadAllAsync(cancellationToken);

            if (!response.Any()) return NotFound();

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ReadAsync([FromRoute] Guid id, CancellationToken cancellationToken = default)
        {
            var response = await _salesService.ReadAsync(id, cancellationToken);

            if (response is null) return NotFound();

            return Ok(response);
        }


        [HttpPut]
        public async Task<IActionResult> PutAsync([FromBody] Sale request, CancellationToken cancellationToken = default)
        {
            var response = await _salesService.UpdateAsync(request, cancellationToken);

            if (!response) return NotFound(response);

            return Ok(response);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync([FromRoute] Guid id, CancellationToken cancellationToken = default)
        {
            var response = await _salesService.DeleteAsync(id, cancellationToken);

            if (!response) return NotFound(response);
            return Ok(response);
        }

    }
}
