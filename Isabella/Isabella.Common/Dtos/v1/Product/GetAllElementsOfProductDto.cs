namespace Isabella.Common.Dtos.Product
{
    using System.Collections.Generic;

    /// <summary>
    /// Imagen de un producto.
    /// </summary>
    public class GetAllElementOfProductDto : GetProductDto
    {
        public List<GetImageProductDto> GetAllImagesProduct { get; set; }
    }
}
