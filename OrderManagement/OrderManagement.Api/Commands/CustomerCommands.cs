using MediatR;
using OrderManagement.Api.Models;

namespace OrderManagement.Api.Commands
{
    /// <summary>
    /// Comando para crear un nuevo cliente en el sistema.
    /// </summary>
    public class CreateCustomerCommand : IRequest<Customer>
    {
        /// <summary>
        /// Nombre completo del cliente.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Dirección de correo electrónico del cliente.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Dirección postal del cliente.
        /// </summary>
        public string Address { get; set; } = string.Empty;

        /// <summary>
        /// Número de teléfono del cliente.
        /// </summary>
        public string Phone { get; set; } = string.Empty;
    }

    /// <summary>
    /// Comando para actualizar la información de un cliente existente.
    /// </summary>
    public class UpdateCustomerCommand : IRequest<bool>
    {
        /// <summary>
        /// Identificador único del cliente a actualizar.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Nombre actualizado del cliente.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Dirección de correo electrónico actualizada del cliente.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Dirección postal actualizada del cliente.
        /// </summary>
        public string Address { get; set; } = string.Empty;

        /// <summary>
        /// Número de teléfono actualizado del cliente.
        /// </summary>
        public string Phone { get; set; } = string.Empty;
    }

    /// <summary>
    /// Comando para eliminar un cliente del sistema.
    /// </summary>
    public class DeleteCustomerCommand : IRequest<bool>
    {
        /// <summary>
        /// Identificador único del cliente a eliminar.
        /// </summary>
        public Guid Id { get; set; }
    }
}
