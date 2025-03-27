using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace OrderManagement.Api.Models.ReadModels
{
    public class OrderReadModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }
        
        public DateTime OrderDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        
        // Información del cliente  
        [BsonRepresentation(BsonType.String)]
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        
        // Ítems de la orden
        public List<OrderItemReadModel> Items { get; set; } = new List<OrderItemReadModel>();
    }

    public class OrderItemReadModel
    {
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        
        // Información del producto
        [BsonRepresentation(BsonType.String)]
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
    }
}
