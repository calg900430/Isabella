namespace Isabella.Web
{
    using AutoMapper;
    using Models.Entities;
    using Isabella.Common.Dtos.SubCategorie;
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
            CreateMap<SubCategorie, GetSubCategorieDto>();
            CreateMap<GetSubCategorieDto, SubCategorie>();

            //Mapeo Gps
            CreateMap<Gps, GetGps>();
            CreateMap<GetGps, Gps>();
            CreateMap<AddGps, Gps>();
            CreateMap<Gps, AddGps>();
        }
    }
}
