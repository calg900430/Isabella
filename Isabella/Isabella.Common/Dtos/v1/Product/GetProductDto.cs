namespace Isabella.Common.Dtos.Product
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Isabella.Common.Dtos.Categorie;
    using Isabella.Common.Dtos.SubCategorie;

    public class GetProductDto
    {
        /// <summary>
        /// Key
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Categoria del producto.
        /// </summary>
        public GetCategorieDto Categorie { get; set; }

        /// <summary>
        /// SubCategorias.
        /// </summary>
        public List<GetSubCategorieDto> GetSubCategories { get; set; }

        /// <summary>
        /// Nombre del Producto ofertado por el Restaurante.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Precio del Produto.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal Price { get; set; }

        /// <summary>
        /// Stock
        /// </summary>
        public int Stock { get; set; }


        /// <summary>
        /// Descripción del Producto.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Promedio de calificaciones del usuario acerca del producto.
        /// </summary>
        public float Average { get; set; }

        /// <summary>
        /// Indica si el producto está disponible.
        /// </summary>
        public bool IsAvailabe { get; set; }

        /// <summary>
        /// Indica si el producto se le puede incluir agregados.
        /// </summary>
        public bool SupportAggregate { get; set; }
    }
}
