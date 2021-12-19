namespace Isabella.Common.Dtos.SubCategorie
{
    public class GetSubCategorieDto
    {
        /// <summary>
        /// Key
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Producto.
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// Nombre de la categoría del producto.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Precio del Produto.
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Enable
        /// </summary>
        public bool IsAvailable { get; set; }

        /// <summary>
        /// Description
        /// </summary>
        public string Description { get; set; }
    }
}
