namespace Isabella.Common.Dtos.Order
{
    using System;
    using System.ComponentModel.DataAnnotations;
    public class DelProduct_PizzaPasta_ToCarShopDto
    {
        /// <summary>
        /// Código de usuario
        /// </summary>
        [Required(ErrorMessage = "Debe introducir su código de usuario.")]
        public string CodeUser { get; set; }

        /// <summary>
        /// ProductId
        /// </summary>
        [Required(ErrorMessage = "Debe especificar el Id del producto.")]
        public int Product_Pizza_PastaId { get; set; }
    }
}
