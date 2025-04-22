using Microsoft.AspNetCore.Mvc;
using Mouts.SalesRecords.Application.Core.Services.Interfaces;
using Mouts.SalesRecords.Domain.Entities;
using Mouts.SalesRecords.Domain.Requests;

namespace Mouts.SalesRecords.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomersController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateCustomerRequest request, CancellationToken cancellationToken = default)
        {
            await _customerService.CreateAsync(request, cancellationToken);
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> ReadAllAsync(CancellationToken cancellationToken = default)
        {
            var response = await _customerService.ReadAllAsync(cancellationToken);

            if (response.Count == 0) return NotFound();

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ReadAsync([FromRoute] Guid id, CancellationToken cancellationToken = default)
        {
            var response = await _customerService.ReadAsync(id, cancellationToken);

            if (response is null) return NotFound();

            return Ok(response);
        }


        [HttpPut]
        public async Task<IActionResult> PutAsync([FromBody] Customer request, CancellationToken cancellationToken = default)
        {
            var response = await _customerService.UpdateAsync(request, cancellationToken);

            if (!response) return NotFound(response);

            return Ok(response);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync([FromRoute] Guid id, CancellationToken cancellationToken = default)
        {
            var response = await _customerService.DeleteAsync(id, cancellationToken);

            if (!response) return NotFound(response);
            return Ok(response);
        }
    }
}
