namespace Isabella.Common.Dtos.ProductAggregate
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using Isabella.Common.Dtos.Category;

    public class GetProductAggregateDto
    {
        /// <summary>
        /// Key
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Categoria del producto.
        /// </summary>
        public GetCategoryDto Category { get; set; }

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
        /// Descripción del Producto.
        /// </summary>
        public string Description { get; set; }
    }
}
