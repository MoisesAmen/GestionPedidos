using MediatR;
using OrderManagement.Api.Models.ReadModels;

namespace OrderManagement.Api.Queries
{
    public class GetOrderByIdQuery : IRequest<OrderReadModel>
    {
        public Guid Id { get; set; }
    }

    public class GetOrdersByCustomerQuery : IRequest<IEnumerable<OrderReadModel>>
    {
        public Guid CustomerId { get; set; }
    }

    public class GetAllOrdersQuery : IRequest<IEnumerable<OrderReadModel>>
    {
        // No se necesitan parámetros adicionales
    }
}
