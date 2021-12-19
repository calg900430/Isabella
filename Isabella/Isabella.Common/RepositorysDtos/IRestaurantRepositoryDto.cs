namespace Isabella.Common.RepositorysDtos
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using Isabella.Common.Dtos.Resturant;

    /// <summary>
    /// Repositorio para el manejo de los Dtos del restaurante
    /// </summary>
    public interface IRestaurantRepositoryDto
    {

        /// <summary>
        /// Cierra el restaurante.
        /// </summary>
        /// <param name="updateRestaurant"></param>
        /// <returns></returns>
        Task<ServiceResponse<bool>> CloseRestautantAsync();

        /// <summary>
        /// Abre el restaurante.
        /// </summary>
        /// <param name="RestaurantId"></param>
        /// <returns></returns>
        Task<ServiceResponse<bool>> OpenRestaurantAsync();

        /// <summary>
        /// Verifica el estado en el que está el restaurante.
        /// </summary>
        /// <param name="RestaurantId"></param>
        /// <returns></returns>
        Task<ServiceResponse<bool>> GetStateRestaurantAsync();
    }
}
