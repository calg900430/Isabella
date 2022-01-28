namespace Isabella.Common.Dtos.Aggregate
{
    using Isabella.Common.Dtos.Aggregate;
    using System.Collections.Generic;

    /// <summary>
    /// Imagen de un producto.
    /// </summary>
    public class GetAllElementOfAggregateDto : GetAggregateDto
    {
        public List<GetImageAggregateDto> GetAllImagesAggregate { get; set; }
    }
}
