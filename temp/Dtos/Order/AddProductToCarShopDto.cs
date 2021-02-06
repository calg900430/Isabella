namespace Isabella.Common.Dtos.Order
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Extras;

    public class AddProductToCarShopDto
    {
        /// <summary>
        /// Código de usuario
        /// </summary>
        [Required(ErrorMessage = "Debe introducir su código de usuario.")]
        [MaxLength(20, ErrorMessage = "Ha superado el limite máximo de caracteres permitidos.")]
        public string CodeUser { get; set; }

        /// <summary>
        /// ProductId
        /// </summary>
        [Required(ErrorMessage = "Debe especificar el Id del producto.")]
        public int ProductId { get; set; }

        /// <summary>
        /// Cantidad de Productos que piensa adquirir el usuario
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:N2}")]
        [Required(ErrorMessage = "Debe introducir la cantidad de productos que desea.")]
        public int Quantity { get; set; }     
    }
}
