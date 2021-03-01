namespace Isabella.Common.Dtos.CarShop
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using Category;
    using SubCategory;

    /// <summary>
    /// Productos en el carrito para pedido un futuro pedido.
    /// </summary>
    public class GetAllProductOfMyCarShopDto
    {
        /// <summary>
        /// Código de indentificación.
        /// </summary>
        public Guid Identification { get; set; }

        /// <summary>
        /// Productos
        /// </summary>
        public List<GetCarShopProductDto> GetCarShopProducts { get; set; }

        /// <summary>
        /// Cantidad de productos sin repetir(o sea productos diferentes) que tiene nuestro carrito.
        /// </summary>
        public int Lines
        {
            get
            {
                return GetCarShopProducts.Count;
            }
        }

        /// <summary>
        /// Precio Total del posible pedido.
        /// </summary>
        public decimal PriceTotal { get; set; }
    }

    /// <summary>
    /// Pedidos de productos standards
    /// </summary>
    public class GetCarShopProductDto
    {
        /// <summary>
        /// Key
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// Nombre del Producto ofertado por el Restaurante.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Precio del Produto.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal Price { get; set; }

        /// <summary>
        /// Descripción del Producto.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Promedio de calificaciones del usuario acerca del producto.
        /// </summary>
        public float Average { get; set; }

        /// <summary>
        /// Indica si el producto está disponible.
        /// </summary>
        public bool IsAvailabe { get; set; }

        /// <summary>
        /// Indica si el producto se le puede incluir agregados.
        /// </summary>
        public bool SupportAggregate { get; set; }

        /// <summary>
        /// Categoria del producto.
        /// </summary>
        public GetCategoryDto Category { get; set; }

        /// <summary>
        /// SubCategoria
        /// </summary>
        public GetSubCategoryDto SubCategory { get; set; }

        /// <summary>
        /// Agregados.
        /// </summary>
        public List<GetCantAggregateDto> CantAggregates { get; set; }

        /// <summary>
        /// Queso Gouda, solo para pizzas y pastas.
        /// </summary>
        public bool? CheeseGouda { get; set; }

        /// <summary>
        /// Cantidad de Productos.
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Precio total del producto, incluye agregados y en caso de pizzas y pastas si es con queso gouda..
        /// </summary>
        public decimal PriceTotal { get; set; }
    }

    public class GetCantAggregateDto
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
        /// Precio del Produto.
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Cantidad deseada.
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Precio total del pedido de este agrego.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal PriceTotal { get; set; }
    }
}
