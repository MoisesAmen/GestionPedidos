using MediatR;
using OrderManagement.Api.Models;

namespace OrderManagement.Api.Commands
{
    public class CreateProductCommand : IRequest<Product>
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
    }

    public class UpdateProductCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
    }

    public class DeleteProductCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
    }
}
