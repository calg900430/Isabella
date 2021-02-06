namespace Isabella.Common.Dtos.CalificationRestaurant
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using Extras;

    public class AddCalificationRestaurantDto
    {
        
        /// <summary>
        /// Código de usuario
        /// </summary>
        [Required(ErrorMessage = "Debe introducir su código de usuario.")]
        [MaxLength(20, ErrorMessage = "Ha superado el limite máximo de caracteres permitidos.")]
        public string CodeUser { get; set; }

        /// <summary>
        /// Calificación del cliente acerca del restaurante
        /// </summary>
        [Required]
        [Range(1, 5, ErrorMessage = "La calificación es de 1 a 5 estrellas.")]
        public EnumCalification Calification { get; set; }

        /// <summary>
        /// Opinion acerca de la calificación del producto.
        /// </summary>
        [Required(ErrorMessage = "Debe emitir el criterio acerca de su calificación.")]
        public string Opinion { get; set; }
    }
}
