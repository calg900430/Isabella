namespace Isabella.Web.Models.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using Isabella.Web.Extras;

    /// <summary>
    /// Entidad que representa los detalles de la orden del usuario.
    /// </summary>
    public class OrderDetail : IModel
    {
        /// <summary>
        /// Key
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Order
        /// </summary>
        public Order Order { get; set; }

        /// <summary>
        /// Producto Standard solicitado por el usuario
        /// </summary>
        public RequestedForUser_ProductStandard RequestedProductStandard { get; set; }

        /// <summary>
        /// Producto Special solicitado por el usuario
        /// </summary>
        public RequestedForUser_ProductSpecial RequestedForUser_ProductSpecial { get; set; }

        /// <summary>
        /// Fecha en que se agregó el producto al carrito de compras
        /// </summary>
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// Fecha en que se actualizo el producto.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? DateUpdate { get; set; }
    }
}
