namespace Isabella.API
{
    using AutoMapper;
    using Models.Entities;
    using Common.Dtos.SubCategory;

    /// <summary>
    /// Perfiles para el AutoMapper
    /// </summary>
    public class AutoMapperProfile : Profile
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public AutoMapperProfile()
        {
            //Mapeo entre las entidades SubCategory y GetSubCategoryDto
            CreateMap<SubCategory, GetSubCategoryDto>();
            CreateMap<GetSubCategoryDto, SubCategory>();
        }
    }
}
