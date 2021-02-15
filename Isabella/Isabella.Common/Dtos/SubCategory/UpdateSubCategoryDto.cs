namespace Isabella.Common.Dtos.SubCategory
{
    /// <summary>
    /// Dto para actualizar una subcategoria.
    /// </summary>
    public class UpdateSubCategoryDto
    {
        /// <summary>
        /// Key
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nombre de la categoría del producto.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Precio del Produto.
        /// </summary>
        public decimal? Price { get; set; }
    }
}
