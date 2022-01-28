namespace Isabella.Web.Models.Entities
{
   
    using System.ComponentModel.DataAnnotations;
    using Models;

    /// <summary>
    /// 
    /// </summary>
    public class SubCategorie : IEntity
    {
        /// <summary>
        /// Key
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Producto.
        /// </summary>
        public Product Product { get; set; }

        /// <summary>
        /// Nombre de la categoría del producto.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Precio del Produto.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal Price { get; set; }

        /// <summary>
        /// Descripción
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Enable
        /// </summary>
        public bool IsAvailable { get; set; }
    }
}
