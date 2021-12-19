namespace Isabella.Common.Dtos.Categorie
{
    /// <summary>
    /// Dto para actualizar una subcategoria.
    /// </summary>
    public class UpdateCategoryDto
    {
        /// <summary>
        /// Key
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nombre de la categoría del producto.
        /// </summary>
        public string Name { get; set; }
    }
}
