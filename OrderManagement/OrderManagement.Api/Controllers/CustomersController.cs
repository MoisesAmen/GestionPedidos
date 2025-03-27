using MediatR;
using Microsoft.AspNetCore.Mvc;
using OrderManagement.Api.Commands;
using OrderManagement.Api.Models;
using OrderManagement.Api.Models.ReadModels;
using OrderManagement.Api.Queries;

namespace OrderManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class CustomersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CustomersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomerReadModel>>> GetAll()
        {
            var customers = await _mediator.Send(new GetAllCustomersQuery());
            return Ok(customers);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerReadModel>> GetById(Guid id)
        {
            var customer = await _mediator.Send(new GetCustomerByIdQuery { Id = id });
            if (customer == null)
                return NotFound();

            return Ok(customer);
        }

        [HttpPost]
        public async Task<ActionResult<Customer>> Create(CreateCustomerCommand command)
        {
            var customer = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = customer.Id }, customer);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, UpdateCustomerCommand command)
        {
            if (id != command.Id)
                return BadRequest();

            var success = await _mediator.Send(command);
            if (!success)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var success = await _mediator.Send(new DeleteCustomerCommand { Id = id });
            if (!success)
                return NotFound();

            return NoContent();
        }
    }
}
