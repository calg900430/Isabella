namespace Isabella.Common.Dtos.Order
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class UpdateCarShopDto
    {
        /// <summary>
        /// Key
        /// </summary>
        [Required(ErrorMessage = "Introduza el Id del producto.")]
        public int ProductId { get; set; }

        /// <summary>
        /// Código de usuario
        /// </summary>
        [Required(ErrorMessage = "Debe introducir su código de usuario.")]
        [MaxLength(20, ErrorMessage = "Ha superado el limite máximo de caracteres permitidos.")]
        public string CodeUser { get; set; }

        /// <summary>
        /// Cantidad de Productos.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:N2}")]
        [Required(ErrorMessage = "Debe introducir la nueva cantidad de productos que desea.")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad de productos seleccionada está fuera del rango permitido.")]
        public int Quantity { get; set; }
    }
}
