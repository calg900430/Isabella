namespace Isabella.Web.ViewModels.ProductViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;

    /// <summary>
    /// AddImageProductViewModel
    /// </summary>
    public class AddImageProductViewModel
    { 
         /// <summary>
         /// Id del Producto.
         /// </summary>
         public int ProductId { get; set; }

         /// <summary>
         /// Imagen
         /// </summary>
         public IFormFile ImageFile { get; set; }
    }
}
