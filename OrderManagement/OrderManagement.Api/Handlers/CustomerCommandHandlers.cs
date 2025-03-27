using MediatR;
using OrderManagement.Api.Commands;
using OrderManagement.Api.Data;
using OrderManagement.Api.Models;
using OrderManagement.Api.Services;

namespace OrderManagement.Api.Handlers
{
    public class CreateCustomerHandler : IRequestHandler<CreateCustomerCommand, Customer>
    {
        private readonly OrderDbContext _context;
        private readonly MongoDbSyncService _syncService;

        public CreateCustomerHandler(OrderDbContext context, MongoDbSyncService syncService)
        {
            _context = context;
            _syncService = syncService;
        }

        public async Task<Customer> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
        {
            var customer = new Customer
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Email = request.Email,
                Address = request.Address,
                Phone = request.Phone
            };

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync(cancellationToken);
            
            // Sincronizar con MongoDB
            await _syncService.SyncCustomerAsync(customer);

            return customer;
        }
    }

    public class UpdateCustomerHandler : IRequestHandler<UpdateCustomerCommand, bool>
    {
        private readonly OrderDbContext _context;
        private readonly MongoDbSyncService _syncService;

        public UpdateCustomerHandler(OrderDbContext context, MongoDbSyncService syncService)
        {
            _context = context;
            _syncService = syncService;
        }

        public async Task<bool> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
        {
            var customer = await _context.Customers.FindAsync(new object[] { request.Id }, cancellationToken);
            if (customer == null)
            {
                return false;
            }

            customer.Name = request.Name;
            customer.Email = request.Email;
            customer.Address = request.Address;
            customer.Phone = request.Phone;

            await _context.SaveChangesAsync(cancellationToken);
            
            // Sincronizar con MongoDB
            await _syncService.SyncCustomerAsync(customer);
            
            return true;
        }
    }

    public class DeleteCustomerHandler : IRequestHandler<DeleteCustomerCommand, bool>
    {
        private readonly OrderDbContext _context;
        private readonly MongoDbContext _mongoContext;

        public DeleteCustomerHandler(OrderDbContext context, MongoDbContext mongoContext)
        {
            _context = context;
            _mongoContext = mongoContext;
        }

        public async Task<bool> Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
        {
            var customer = await _context.Customers.FindAsync(new object[] { request.Id }, cancellationToken);
            if (customer == null)
            {
                return false;
            }

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync(cancellationToken);
            
            // Eliminar de MongoDB
            var filter = MongoDB.Driver.Builders<Models.ReadModels.CustomerReadModel>.Filter.Eq(c => c.Id, request.Id);
            await _mongoContext.Customers.DeleteOneAsync(filter, cancellationToken);
            
            return true;
        }
    }
}
