namespace Isabella.Common.Dtos.Order
{
    using System;
    using System.ComponentModel.DataAnnotations;
    public class DelProductToCarShopDto
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


    }
}
