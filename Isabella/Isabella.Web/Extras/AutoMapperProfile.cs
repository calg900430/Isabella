namespace Isabella.Web.Extras
{
    using AutoMapper;

    using Common.Dtos.Users;
    //using Common.Dtos.Product;
    using Models;
    using Models.Entities;

    /// <summary>
    /// Crea mapas para las asignaciones entre los Dtos y las entidades.
    /// </summary>
    public class AutoMapperProfile : Profile
    {
        /// <summary>
        /// Implementa los mapas de un entidad a otra.
        /// </summary>
        public AutoMapperProfile()
        {
            //Mapeo entre la entidad User y sus Dtos
            CreateMap<RegisterUserDto, User>();
            CreateMap<User, RegisterUserDto>();
            CreateMap<User, GetUserDto>();
            CreateMap<GetUserDto, User>();
            CreateMap<UpdateUserDto, User>();
            CreateMap<User, UpdateUserDto>();
        }
    }
}
