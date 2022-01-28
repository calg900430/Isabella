namespace Isabella.Web.ViewModels.AggregateViewModel
{
    
    using Isabella.Common.Dtos.Aggregate;
    using System.Collections.Generic;

    /// <summary>
    /// ViewModel para el manejo de un producto y sus entidades.
    /// </summary>
    public class GetAggregateViewModel : GetAggregateDto
    {
        /// <summary>
        /// Imagenes
        /// </summary>
        public List<GetImageAggregateDto> GetAllImagesAggregate { get; set; }
    }
}
