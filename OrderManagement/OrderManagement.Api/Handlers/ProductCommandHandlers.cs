using MediatR;
using OrderManagement.Api.Commands;
using OrderManagement.Api.Data;
using OrderManagement.Api.Models;
using OrderManagement.Api.Services;

namespace OrderManagement.Api.Handlers
{
    public class CreateProductHandler : IRequestHandler<CreateProductCommand, Product>
    {
        private readonly OrderDbContext _context;
        private readonly MongoDbSyncService _syncService;

        public CreateProductHandler(OrderDbContext context, MongoDbSyncService syncService)
        {
            _context = context;
            _syncService = syncService;
        }

        public async Task<Product> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var product = new Product
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                StockQuantity = request.StockQuantity
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync(cancellationToken);
            
            // Sincronizar con MongoDB
            await _syncService.SyncProductAsync(product);

            return product;
        }
    }

    public class UpdateProductHandler : IRequestHandler<UpdateProductCommand, bool>
    {
        private readonly OrderDbContext _context;
        private readonly MongoDbSyncService _syncService;

        public UpdateProductHandler(OrderDbContext context, MongoDbSyncService syncService)
        {
            _context = context;
            _syncService = syncService;
        }

        public async Task<bool> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _context.Products.FindAsync(new object[] { request.Id }, cancellationToken);
            if (product == null)
            {
                return false;
            }

            product.Name = request.Name;
            product.Description = request.Description;
            product.Price = request.Price;
            product.StockQuantity = request.StockQuantity;

            await _context.SaveChangesAsync(cancellationToken);
            
            // Sincronizar con MongoDB
            await _syncService.SyncProductAsync(product);
            
            return true;
        }
    }

    public class DeleteProductHandler : IRequestHandler<DeleteProductCommand, bool>
    {
        private readonly OrderDbContext _context;
        private readonly MongoDbContext _mongoContext;

        public DeleteProductHandler(OrderDbContext context, MongoDbContext mongoContext)
        {
            _context = context;
            _mongoContext = mongoContext;
        }

        public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _context.Products.FindAsync(new object[] { request.Id }, cancellationToken);
            if (product == null)
            {
                return false;
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync(cancellationToken);
            
            // Eliminar de MongoDB
            var filter = MongoDB.Driver.Builders<Models.ReadModels.ProductReadModel>.Filter.Eq(p => p.Id, request.Id);
            await _mongoContext.Products.DeleteOneAsync(filter, cancellationToken);
            
            return true;
        }
    }
}
