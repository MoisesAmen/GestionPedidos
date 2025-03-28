using MediatR;
using OrderManagement.Api.Models;

namespace OrderManagement.Api.Commands
{
    /// <summary>
    /// Comando para crear un nuevo producto en el sistema.
    /// </summary>
    public class CreateProductCommand : IRequest<Product>
    {
        /// <summary>
        /// Nombre del producto.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Descripción detallada del producto.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Precio de venta del producto.
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Cantidad disponible en inventario.
        /// </summary>
        public int StockQuantity { get; set; }
    }

    /// <summary>
    /// Comando para actualizar la información de un producto existente.
    /// </summary>
    public class UpdateProductCommand : IRequest<bool>
    {
        /// <summary>
        /// Identificador único del producto a actualizar.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Nombre actualizado del producto.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Descripción actualizada del producto.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Precio actualizado del producto.
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Cantidad actualizada en inventario.
        /// </summary>
        public int StockQuantity { get; set; }
    }

    /// <summary>
    /// Comando para eliminar un producto del sistema.
    /// </summary>
    public class DeleteProductCommand : IRequest<bool>
    {
        /// <summary>
        /// Identificador único del producto a eliminar.
        /// </summary>
        public Guid Id { get; set; }
    }
}
