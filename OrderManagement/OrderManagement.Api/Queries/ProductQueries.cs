using MediatR;
using OrderManagement.Api.Models.ReadModels;

namespace OrderManagement.Api.Queries
{
    public class GetProductByIdQuery : IRequest<ProductReadModel>
    {
        public Guid Id { get; set; }
    }

    public class GetAllProductsQuery : IRequest<IEnumerable<ProductReadModel>>
    {
        // No se necesitan parámetros adicionales
    }
}
