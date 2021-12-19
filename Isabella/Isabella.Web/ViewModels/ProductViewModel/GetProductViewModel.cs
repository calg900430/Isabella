namespace Isabella.Web.ViewModels.ProductViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Isabella.Common.Dtos.Aggregate;
    using Isabella.Common.Dtos.Product;

    /// <summary>
    /// ViewModel para el manejo de un producto y sus entidades.
    /// </summary>
    public class GetProductViewModel: GetProductDto
    {
        /// <summary>
        /// Imagenes
        /// </summary>
        public List<GetImageProductDto> GetAllImagesProducts { get; set; }
    }
}
