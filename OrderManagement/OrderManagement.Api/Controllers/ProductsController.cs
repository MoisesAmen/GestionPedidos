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
    public class ProductsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductReadModel>>> GetAll()
        {
            var products = await _mediator.Send(new GetAllProductsQuery());
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductReadModel>> GetById(Guid id)
        {
            var product = await _mediator.Send(new GetProductByIdQuery { Id = id });
            if (product == null)
                return NotFound();

            return Ok(product);
        }

        [HttpPost]
        public async Task<ActionResult<Product>> Create(CreateProductCommand command)
        {
            var product = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, UpdateProductCommand command)
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
            var success = await _mediator.Send(new DeleteProductCommand { Id = id });
            if (!success)
                return NotFound();

            return NoContent();
        }
    }
}
