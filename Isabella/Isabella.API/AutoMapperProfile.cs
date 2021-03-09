namespace Isabella.API
{
    using AutoMapper;
    using Models.Entities;
    using Common.Dtos.SubCategory;
    using Isabella.Common.Dtos.Order;

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

            //Mapeo Gps
            CreateMap<Gps, GetGps>();
            CreateMap<GetGps, Gps>();
            CreateMap<AddGps, Gps>();
            CreateMap<Gps, AddGps>();
        }
    }
}
