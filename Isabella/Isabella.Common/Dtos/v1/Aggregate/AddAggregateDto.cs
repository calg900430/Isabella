namespace Isabella.Common.Dtos.Aggregate
{
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Agrega un nuevo producto standard.
    /// </summary>
    public class AddAggregateDto
    {
      
        /// <summary>
        /// Nombre 
        /// </summary>
        [JsonProperty("Name")]
        [Required(ErrorMessage = "Debe introducir el nombre del producto.")]
        [MaxLength(100, ErrorMessage = "Ha superado el limite máximo de caracteres permitidos para el nombre del producto.")]
        public string Name { get; set; }

        /// <summary>
        /// Precio del Producto
        /// </summary>
        [JsonProperty("Price")]
        [Required(ErrorMessage = "Debe introducir el precio del producto.")]
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal Price { get; set; }

        /// <summary>
        /// Indica si el producto está disponible.
        /// </summary>
        [JsonProperty("IsAvailable")]
        public bool IsAvailabe { get; set; } = true;

        /// <summary>
        /// Cantidad disponible en el Stock
        /// </summary>
        [Required(ErrorMessage = "Debe introducir la cantidad de productos.")]
        public int Stock { get; set; }

        /// <summary>
        /// Descripción de la publicacion
        /// </summary>
        [MaxLength(1000, ErrorMessage = "Ha superado el limite máximo de caracteres permitidos para dar una descripción del producto.")]
        public string Description { get; set; }
    }
}
