using MediatR;
using OrderManagement.Api.Models;

namespace OrderManagement.Api.Commands
{
    public class CreateCustomerCommand : IRequest<Customer>
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
    }

    public class UpdateCustomerCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
    }

    public class DeleteCustomerCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
    }
}
