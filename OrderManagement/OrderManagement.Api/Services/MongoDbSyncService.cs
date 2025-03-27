using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using OrderManagement.Api.Data;
using OrderManagement.Api.Models;
using OrderManagement.Api.Models.ReadModels;

namespace OrderManagement.Api.Services
{
    public class MongoDbSyncService
    {
        private readonly OrderDbContext _dbContext;
        private readonly MongoDbContext _mongoContext;

        public MongoDbSyncService(OrderDbContext dbContext, MongoDbContext mongoContext)
        {
            _dbContext = dbContext;
            _mongoContext = mongoContext;
        }

        public async Task SyncCustomerAsync(Customer customer)
        {
            var readModel = new CustomerReadModel
            {
                Id = customer.Id,
                Name = customer.Name,
                Email = customer.Email,
                Address = customer.Address,
                Phone = customer.Phone
            };

            var filter = Builders<CustomerReadModel>.Filter.Eq(c => c.Id, customer.Id);
            await _mongoContext.Customers.ReplaceOneAsync(filter, readModel, new ReplaceOptions { IsUpsert = true });
        }

        public async Task SyncProductAsync(Product product)
        {
            var readModel = new ProductReadModel
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                StockQuantity = product.StockQuantity
            };

            var filter = Builders<ProductReadModel>.Filter.Eq(p => p.Id, product.Id);
            await _mongoContext.Products.ReplaceOneAsync(filter, readModel, new ReplaceOptions { IsUpsert = true });
        }

        public async Task SyncOrderAsync(Order order)
        {
            // Cargar datos relacionados si no están ya cargados
            if (order.Customer == null)
            {
                await _dbContext.Entry(order).Reference(o => o.Customer).LoadAsync();
            }
            
            if (!order.Items.Any())
            {
                await _dbContext.Entry(order).Collection(o => o.Items).LoadAsync();
                
                foreach (var item in order.Items)
                {
                    await _dbContext.Entry(item).Reference(i => i.Product).LoadAsync();
                }
            }

            var readModel = new OrderReadModel
            {
                Id = order.Id,
                OrderDate = order.OrderDate,
                Status = order.Status.ToString(),
                TotalAmount = order.TotalAmount,
                CustomerId = order.CustomerId,
                CustomerName = order.Customer?.Name ?? string.Empty,
                CustomerEmail = order.Customer?.Email ?? string.Empty,
                Items = order.Items.Select(item => new OrderItemReadModel
                {
                    Id = item.Id,
                    ProductId = item.ProductId,
                    ProductName = item.Product.Name,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice
                }).ToList()
            };

            var filter = Builders<OrderReadModel>.Filter.Eq(o => o.Id, order.Id);
            await _mongoContext.Orders.ReplaceOneAsync(filter, readModel, new ReplaceOptions { IsUpsert = true });
        }
    }
}
