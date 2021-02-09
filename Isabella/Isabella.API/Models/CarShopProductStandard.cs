namespace Isabella.API.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Carro de compras Fast(Para pedidos informales, o sea usuarios que no desean ser clientes oficiales)
    /// </summary>
    public class CarShopProductStandard
    {
        /// <summary>
        /// Key
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Código de identificación.
        /// </summary>
        public CodeIdentification CodeIdentification {get; set; }

        /// <summary>
        /// Producto Standard
        /// </summary>
        public ProductStandard ProductStandard { get; set; }

        /// <summary>
        /// Precio actual del Producto.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal Price { get; set; }

        /// <summary>
        /// Cantidad de Productos.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public int Quantity { get; set; }

        /// <summary>
        /// Precio total del Producto.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal PriceTotal { get { return this.Price * (decimal)this.Quantity; } }
    }
}
