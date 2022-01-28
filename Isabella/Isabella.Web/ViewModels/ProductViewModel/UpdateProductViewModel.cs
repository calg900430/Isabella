namespace Isabella.Web.ViewModels.ProductViewModel
{
    using Isabella.Common.Dtos.Product;
    using Microsoft.AspNetCore.Http;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
   

    /// <summary>
    /// UpdateProductViewModel
    /// </summary>
    public class UpdateProductViewModel : UpdateProductDto
    {
        /// <summary>
        /// Imagen a actualizar
        /// </summary>
        [Display(Name = "Image")]
        public IFormFile ImageFile { get; set; }

        /// <summary>
        /// Imagenes
        /// </summary>
        public List<GetImageProductDto> Images { get; set; }
    }
}
