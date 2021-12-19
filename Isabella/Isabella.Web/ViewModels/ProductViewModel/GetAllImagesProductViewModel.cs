namespace Isabella.Web.ViewModels.ProductViewModel
{
    using Isabella.Common.Dtos.Product;
    using Microsoft.AspNetCore.Http;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// ViewModel de las imagenes de los productos
    /// </summary>
    public class GetAllImagesProductViewModel
    {
        /// <summary>
        /// Producto
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nueva Imagen
        /// </summary>
        public IFormFile ImageFile { get; set; }

        /// <summary>
        /// Imagenes
        /// </summary>
        public List<GetImageProductDto> GetImages { get; set; }
    }
}
