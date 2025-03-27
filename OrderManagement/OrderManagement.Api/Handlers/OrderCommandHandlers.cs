using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderManagement.Api.Commands;
using OrderManagement.Api.Data;
using OrderManagement.Api.Models;
using OrderManagement.Api.Services;

namespace OrderManagement.Api.Handlers
{
    public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, Order>
    {
        private readonly OrderDbContext _context;
        private readonly MongoDbSyncService _syncService;

        public CreateOrderHandler(OrderDbContext context, MongoDbSyncService syncService)
        {
            _context = context;
            _syncService = syncService;
        }

        public async Task<Order> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            // Verificar que el cliente exista
            var customer = await _context.Customers.FindAsync(new object[] { request.CustomerId }, cancellationToken);
            if (customer == null)
            {
                throw new Exception($"Cliente con ID {request.CustomerId} no encontrado.");
            }

            // Crear la orden
            var order = new Order
            {
                Id = Guid.NewGuid(),
                CustomerId = request.CustomerId,
                OrderDate = DateTime.UtcNow,
                Status = OrderStatus.Pending,
                TotalAmount = 0, // Se calculará basado en los ítems
                Customer = customer // Asignar el cliente para facilitar la sincronización
            };

            // Recopilar los IDs de productos para buscarlos de una sola vez
            var productIds = request.Items.Select(i => i.ProductId).ToList();
            var products = await _context.Products
                .Where(p => productIds.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id, cancellationToken);

            // Crear los ítems de la orden y calcular el total
            foreach (var item in request.Items)
            {
                if (!products.TryGetValue(item.ProductId, out var product))
                {
                    throw new Exception($"Producto con ID {item.ProductId} no encontrado.");
                }

                if (product.StockQuantity < item.Quantity)
                {
                    throw new Exception($"Stock insuficiente para el producto {product.Name}.");
                }

                var orderItem = new OrderItem
                {
                    Id = Guid.NewGuid(),
                    OrderId = order.Id,
                    ProductId = product.Id,
                    Quantity = item.Quantity,
                    UnitPrice = product.Price,
                    Product = product // Asignar el producto para facilitar la sincronización
                };

                order.Items.Add(orderItem);
                order.TotalAmount += orderItem.UnitPrice * orderItem.Quantity;

                // Actualizar el stock
                product.StockQuantity -= item.Quantity;
            }

            // Guardar la orden
            _context.Orders.Add(order);
            await _context.SaveChangesAsync(cancellationToken);
            
            // Sincronizar con MongoDB
            await _syncService.SyncOrderAsync(order);

            return order;
        }
    }

    public class UpdateOrderStatusHandler : IRequestHandler<UpdateOrderStatusCommand, bool>
    {
        private readonly OrderDbContext _context;

        public UpdateOrderStatusHandler(OrderDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
        {
            var order = await _context.Orders.FindAsync(new object[] { request.OrderId }, cancellationToken);
            if (order == null)
            {
                return false;
            }

            order.Status = request.NewStatus;
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }

    public class CancelOrderHandler : IRequestHandler<CancelOrderCommand, bool>
    {
        private readonly OrderDbContext _context;

        public CancelOrderHandler(OrderDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await _context.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken);

            if (order == null)
            {
                return false;
            }

            if (order.Status == OrderStatus.Delivered)
            {
                throw new Exception("No se puede cancelar una orden que ya fue entregada.");
            }

            // Si la orden ya está cancelada, no hacemos nada
            if (order.Status == OrderStatus.Cancelled)
            {
                return true;
            }

            // Restaurar el stock
            foreach (var item in order.Items)
            {
                item.Product.StockQuantity += item.Quantity;
            }

            order.Status = OrderStatus.Cancelled;
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
