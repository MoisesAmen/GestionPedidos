using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using OrderManagement.Api.Models.ReadModels;

namespace OrderManagement.Api.Data
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(IConfiguration configuration)
        {
            var client = new MongoClient(configuration.GetConnectionString("MongoDbConnection"));
            _database = client.GetDatabase(configuration["ConnectionStrings:MongoDbDatabase"]);
        }

        public IMongoCollection<CustomerReadModel> Customers => _database.GetCollection<CustomerReadModel>("Customers");
        public IMongoCollection<ProductReadModel> Products => _database.GetCollection<ProductReadModel>("Products");
        public IMongoCollection<OrderReadModel> Orders => _database.GetCollection<OrderReadModel>("Orders");
    }
}
