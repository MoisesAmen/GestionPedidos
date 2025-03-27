namespace OrderManagement.Api.Models
{
    public class OrderItem
    {
        public Guid Id { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        
        // Relaciones
        public Guid OrderId { get; set; }
        public virtual Order Order { get; set; } = null!;
        
        public Guid ProductId { get; set; }
        public virtual Product Product { get; set; } = null!;
    }
}
