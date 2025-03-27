using MediatR;
using MongoDB.Driver;
using OrderManagement.Api.Data;
using OrderManagement.Api.Models.ReadModels;
using OrderManagement.Api.Queries;

namespace OrderManagement.Api.Handlers
{
    public class GetCustomerByIdHandler : IRequestHandler<GetCustomerByIdQuery, CustomerReadModel>
    {
        private readonly MongoDbContext _context;

        public GetCustomerByIdHandler(MongoDbContext context)
        {
            _context = context;
        }

        public async Task<CustomerReadModel> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
        {
            var filter = Builders<CustomerReadModel>.Filter.Eq(c => c.Id, request.Id);
            return await _context.Customers.Find(filter).FirstOrDefaultAsync(cancellationToken);
        }
    }

    public class GetAllCustomersHandler : IRequestHandler<GetAllCustomersQuery, IEnumerable<CustomerReadModel>>
    {
        private readonly MongoDbContext _context;

        public GetAllCustomersHandler(MongoDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CustomerReadModel>> Handle(GetAllCustomersQuery request, CancellationToken cancellationToken)
        {
            return await _context.Customers.Find(_ => true).ToListAsync(cancellationToken);
        }
    }
}
