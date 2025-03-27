using MediatR;
using OrderManagement.Api.Models;

namespace OrderManagement.Api.Commands
{
    public class CreateOrderCommand : IRequest<Order>
    {
        public Guid CustomerId { get; set; }
        public List<OrderItemDto> Items { get; set; } = new List<OrderItemDto>();
        
        public class OrderItemDto
        {
            public Guid ProductId { get; set; }
            public int Quantity { get; set; }
        }
    }

    public class UpdateOrderStatusCommand : IRequest<bool>
    {
        public Guid OrderId { get; set; }
        public OrderStatus NewStatus { get; set; }
    }

    public class CancelOrderCommand : IRequest<bool>
    {
        public Guid OrderId { get; set; }
    }
}
