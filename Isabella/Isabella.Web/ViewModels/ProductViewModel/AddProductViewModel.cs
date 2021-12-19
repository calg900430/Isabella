namespace Isabella.Web.ViewModels.ProductViewModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;

    using Isabella.Common.Dtos.Product;
   
    /// <summary>
    /// AddProductViewModel
    /// </summary>
    public class AddProductViewModel : AddProductDto
    {
        [Display(Name = "Image")]
        public IFormFile ImageFile { get; set; }
    }
}
