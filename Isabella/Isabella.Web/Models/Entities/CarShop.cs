namespace Isabella.Web.Models.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Isabella.Common.Extras;
    using Isabella.Web.Extras;

    /// <summary>
    /// Entidad que representa el carrito de compras del usuario
    /// </summary>
    public class CarShop : IModel
    {
        /// <summary>
        /// Key
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Usuario
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// Producto Standard
        /// </summary>
        public RequestedForUser_ProductStandard RequestedProductStandard { get; set; }

        /// <summary>
        /// Producto Special
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
