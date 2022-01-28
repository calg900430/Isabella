namespace Isabella.Web.ViewModels.ProductViewModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    using Isabella.Common.Dtos.Product;
   

    /// <summary>
    /// Agregar un nuevo producto.
    /// </summary>
    public class AddProductViewModel : AddProductDto
    {
        /// <summary>
        /// Imagen
        /// </summary>
        [Required(ErrorMessage = "Es necesario una imagen para el producto.")]
        public IFormFile ImageFile { get; set; }
    }
}
