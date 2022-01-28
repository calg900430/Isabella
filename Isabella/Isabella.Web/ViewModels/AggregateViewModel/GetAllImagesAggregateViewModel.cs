namespace Isabella.Web.ViewModels.AggregateViewModel
{
    
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Isabella.Common.Dtos.Aggregate;
    using Microsoft.AspNetCore.Http;

    /// <summary>
    /// ViewModel de las imagenes de los productos
    /// </summary>
    public class GetAllImagesAggregateViewModel
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
        public List<GetImageAggregateDto> GetImages { get; set; }
    }
}
