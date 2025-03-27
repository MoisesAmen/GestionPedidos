namespace OrderManagement.Api.Models
{
    public class Order
    {
        public Guid Id { get; set; }
        public DateTime OrderDate { get; set; }
        public OrderStatus Status { get; set; }
        public decimal TotalAmount { get; set; }
        
        // Relaciones
        public Guid CustomerId { get; set; }
        public virtual Customer Customer { get; set; } = null!;
        
        public virtual ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    }
}
