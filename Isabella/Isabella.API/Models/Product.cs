namespace Isabella.API.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using Extras;

    /// <summary>
    /// Entidad producto: Son los platos que oferta el Restaurante.
    /// </summary>
    public class Product : IModel
    {
        /// <summary>
        /// Key
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Categoria del producto.
        /// </summary>
        public Category Category { get; set; }



        /// <summary>
        /// Nombre del Producto ofertado por el Restaurante.
        /// </summary>
        [MaxLength(50, ErrorMessage = "El nombre no puede exceder de los 50 caracteres.")]
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Precio del Produto.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal Price { get; set; }

        /// <summary>
        /// Ultima Compra vez que algún usuario compro el producto.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? LastBuy { get; set; }

        /// <summary>
        /// Fecha en que se creo el producto
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? DateCreated { get; set; }

        /// <summary>
        /// Fecha en que se actualizo el producto.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? DateUpdate { get; set; }

        /// <summary>
        /// Indica si el producto está disponible.
        /// </summary>
        public bool IsAvailabe { get; set; } = true;

        /// <summary>
        /// Cantidad disponible en el Stock
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = false)]
        public int Stock { get; set; }

        /// <summary>
        /// Descripción del Producto.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Promedio de calificaciones del usuario acerca del producto.
        /// </summary>
        public float Average { get; set; }

    }
}
