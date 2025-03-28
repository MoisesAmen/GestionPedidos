using MediatR;
using OrderManagement.Api.Models;

namespace OrderManagement.Api.Commands
{
    /// <summary>
    /// Comando para crear un nuevo pedido en el sistema.
    /// </summary>
    public class CreateOrderCommand : IRequest<Order>
    {
        /// <summary>
        /// Identificador único del cliente que realiza el pedido.
        /// </summary>
        public Guid CustomerId { get; set; }

        /// <summary>
        /// Lista de productos incluidos en el pedido.
        /// </summary>
        public List<OrderItemDto> Items { get; set; } = new List<OrderItemDto>();
        
        /// <summary>
        /// Representa un ítem individual dentro de un pedido.
        /// </summary>
        public class OrderItemDto
        {
            /// <summary>
            /// Identificador único del producto solicitado.
            /// </summary>
            public Guid ProductId { get; set; }

            /// <summary>
            /// Cantidad solicitada del producto.
            /// </summary>
            public int Quantity { get; set; }
        }
    }

    /// <summary>
    /// Comando para actualizar el estado de un pedido existente.
    /// </summary>
    public class UpdateOrderStatusCommand : IRequest<bool>
    {
        /// <summary>
        /// Identificador único del pedido a actualizar.
        /// </summary>
        public Guid OrderId { get; set; }

        /// <summary>
        /// Nuevo estado que se asignará al pedido.
        /// </summary>
        public OrderStatus NewStatus { get; set; }
    }

    /// <summary>
    /// Comando para cancelar un pedido existente.
    /// </summary>
    public class CancelOrderCommand : IRequest<bool>
    {
        /// <summary>
        /// Identificador único del pedido a cancelar.
        /// </summary>
        public Guid OrderId { get; set; }
    }
}
