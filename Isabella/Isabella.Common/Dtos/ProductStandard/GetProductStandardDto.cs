namespace Isabella.Common.Dtos.ProductStandard
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Isabella.Common.Dtos.CategoryProductStandard;
   
    public class GetProductStandardDto
    {
        /// <summary>
        /// Key
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Categoria del producto.
        /// </summary>
        public GetCategoryProductStandardDto Category { get; set; }

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

        /// <summary>
        /// Promedio de calificaciones del usuario acerca del producto.
        /// </summary>
        public float Average { get; set; }
    }
}
