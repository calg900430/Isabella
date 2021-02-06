namespace Isabella.Common.Dtos.Product
{
    using System;
    using System.Collections.Generic;

    public class GetProductDto
    {

        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Nombre del Producto ofertado por el Restaurante.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Categoria
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        ///  Precio del Produto.
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Indica si el producto está disponible.
        /// </summary>
        public bool IsAvailabe { get; set; } = true;

        /// <summary>
        /// Cantidad.
        /// </summary>
        public int Stock { get; set; }

        /// <summary>
        /// Descripción del Producto.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Promedio de calificaciones del usuario.
        /// </summary>
        public float Average { get; set; }

        /// <summary>
        /// Imagenes del producto
        /// </summary>
        public List<GetProductImageDto> GetProductImage { get; set; }
    }
}
