using MediatR;
using OrderManagement.Api.Models.ReadModels;

namespace OrderManagement.Api.Queries
{
    public class GetCustomerByIdQuery : IRequest<CustomerReadModel>
    {
        public Guid Id { get; set; }
    }

    public class GetAllCustomersQuery : IRequest<IEnumerable<CustomerReadModel>>
    {
        // No se necesitan parámetros adicionales
    }
}
