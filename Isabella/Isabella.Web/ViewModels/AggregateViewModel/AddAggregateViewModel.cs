namespace Isabella.Web.ViewModels.AggregateViewModel
{
    using System.ComponentModel.DataAnnotations;
    using Microsoft.AspNetCore.Http;
    using Isabella.Common.Dtos.Aggregate;

    /// <summary>
    /// AddProductViewModel
    /// </summary>
    public class AddAggregateViewModel : AddAggregateDto
    {
        /// <summary>
        /// Imagen del Agregado
        /// </summary>
        [Display(Name = "Image")]
        public IFormFile ImageFile { get; set; }
    }
}
