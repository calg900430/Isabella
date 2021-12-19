namespace Isabella.Common.Dtos.Product
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using Common;

    /// <summary>
    /// Emite una calificación sobre un producto.
    /// </summary>
    public class AddCalificationProductDto
    {
       
        /// <summary>
        /// Id del Producto.
        /// </summary>
        [Required(ErrorMessage = "Debe introducir el Id del producto.")]
        public int ProductId { get; set; }

        /// <summary>
        /// Calificación del cliente acerca del producto.
        /// </summary>
        [Required]
        [Range(1, 5, ErrorMessage = "La calificación es de 1 a 5 estrellas.")]
        public CommonConstants.EnumCalification Calification { get; set; }

        /// <summary>
        /// Opinion acerca de la calificación del producto.
        /// </summary>
        [Required(ErrorMessage = "Debe emitir el criterio acerca del motivo de su calificación.")]
        [MaxLength(1000, ErrorMessage = "Ha superado el limite máximo de caracteres permitidos para dar" +
        " un criterio sobre del motivo de su calificación.")]
        public string Opinion { get; set; }
    }
}
