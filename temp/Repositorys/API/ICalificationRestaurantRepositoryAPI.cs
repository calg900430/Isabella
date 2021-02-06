namespace Isabella.Web.Repositorys.API
{
    using System.Threading.Tasks;
    using Common;   
    using Common.Dtos.CalificationRestaurant;
    using Models.Entities;
    using Services;

    /// <summary>
    /// Servicio para la calificación del restaurante.
    /// </summary>
    public interface ICalificationRestaurantRepositoryAPI: IGenericRepository<CalificationRestaurant>
    {
        /// <summary>
        /// Agrega una calificación acerca del restaurante.
        /// </summary>
        /// <param name="addCalificationRestaurant"></param>
        /// <returns></returns>
        Task<ServiceResponse<bool>> AddCalificationForRestaurantAsync(AddCalificationRestaurantDto addCalificationRestaurant);
    }
}
