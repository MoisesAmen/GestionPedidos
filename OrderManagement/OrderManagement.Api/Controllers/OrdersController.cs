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
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OrdersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderReadModel>>> GetAll()
        {
            var orders = await _mediator.Send(new GetAllOrdersQuery());
            return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderReadModel>> GetById(Guid id)
        {
            var order = await _mediator.Send(new GetOrderByIdQuery { Id = id });
            if (order == null)
                return NotFound();

            return Ok(order);
        }

        [HttpGet("customer/{customerId}")]
        public async Task<ActionResult<IEnumerable<OrderReadModel>>> GetByCustomer(Guid customerId)
        {
            var orders = await _mediator.Send(new GetOrdersByCustomerQuery { CustomerId = customerId });
            return Ok(orders);
        }

        [HttpPost]
        public async Task<ActionResult<Order>> Create(CreateOrderCommand command)
        {
            try
            {
                var order = await _mediator.Send(command);
                return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] OrderStatus newStatus)
        {
            var command = new UpdateOrderStatusCommand { OrderId = id, NewStatus = newStatus };
            
            var success = await _mediator.Send(command);
            if (!success)
                return NotFound();

            return NoContent();
        }

        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> Cancel(Guid id)
        {
            try
            {
                var success = await _mediator.Send(new CancelOrderCommand { OrderId = id });
                if (!success)
                    return NotFound();

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
