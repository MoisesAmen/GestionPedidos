namespace OrderManagement.Api.Models
{
    public class Customer
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        
        // Propiedades de navegación
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
