using MediatR;
using MongoDB.Driver;
using OrderManagement.Api.Data;
using OrderManagement.Api.Models.ReadModels;
using OrderManagement.Api.Queries;

namespace OrderManagement.Api.Handlers
{
    public class GetOrderByIdHandler : IRequestHandler<GetOrderByIdQuery, OrderReadModel>
    {
        private readonly MongoDbContext _context;

        public GetOrderByIdHandler(MongoDbContext context)
        {
            _context = context;
        }

        public async Task<OrderReadModel> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
        {
            var filter = Builders<OrderReadModel>.Filter.Eq(o => o.Id, request.Id);
            return await _context.Orders.Find(filter).FirstOrDefaultAsync(cancellationToken);
        }
    }

    public class GetOrdersByCustomerHandler : IRequestHandler<GetOrdersByCustomerQuery, IEnumerable<OrderReadModel>>
    {
        private readonly MongoDbContext _context;

        public GetOrdersByCustomerHandler(MongoDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<OrderReadModel>> Handle(GetOrdersByCustomerQuery request, CancellationToken cancellationToken)
        {
            var filter = Builders<OrderReadModel>.Filter.Eq(o => o.CustomerId, request.CustomerId);
            return await _context.Orders.Find(filter).ToListAsync(cancellationToken);
        }
    }

    public class GetAllOrdersHandler : IRequestHandler<GetAllOrdersQuery, IEnumerable<OrderReadModel>>
    {
        private readonly MongoDbContext _context;

        public GetAllOrdersHandler(MongoDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<OrderReadModel>> Handle(GetAllOrdersQuery request, CancellationToken cancellationToken)
        {
            return await _context.Orders.Find(_ => true).ToListAsync(cancellationToken);
        }
    }
}
