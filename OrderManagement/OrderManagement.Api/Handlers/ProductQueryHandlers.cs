using MediatR;
using MongoDB.Driver;
using OrderManagement.Api.Data;
using OrderManagement.Api.Models.ReadModels;
using OrderManagement.Api.Queries;

namespace OrderManagement.Api.Handlers
{
    public class GetProductByIdHandler : IRequestHandler<GetProductByIdQuery, ProductReadModel>
    {
        private readonly MongoDbContext _context;

        public GetProductByIdHandler(MongoDbContext context)
        {
            _context = context;
        }

        public async Task<ProductReadModel> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            var filter = Builders<ProductReadModel>.Filter.Eq(p => p.Id, request.Id);
            return await _context.Products.Find(filter).FirstOrDefaultAsync(cancellationToken);
        }
    }

    public class GetAllProductsHandler : IRequestHandler<GetAllProductsQuery, IEnumerable<ProductReadModel>>
    {
        private readonly MongoDbContext _context;

        public GetAllProductsHandler(MongoDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProductReadModel>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
        {
            return await _context.Products.Find(_ => true).ToListAsync(cancellationToken);
        }
    }
}
