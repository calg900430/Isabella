namespace Isabella.API.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Carro de compras Fast(Para pedidos informales, o sea usuarios que no desean ser clientes oficiales)
    /// </summary>
    public class CarShop
    {
        /// <summary>
        /// Key
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Código de identificación.
        /// </summary>
        public CodeIdentification CodeVerification {get; set; }

        /// <summary>
        /// Producto Standard
        /// </summary>
        public RequestedProductStandard RequestedProductStandard { get; set; }

        /// <summary>
        /// Producto Special
        /// </summary>
        public RequestedProductSpecial RequestedProductSpecial { get; set; }
    }
}
