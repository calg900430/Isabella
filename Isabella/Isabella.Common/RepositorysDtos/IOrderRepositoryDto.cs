namespace Isabella.Common.RepositorysDtos
{
    using Isabella.Common.Dtos.Order;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Repositorio para el manejo de las ordenes(Pedidos)
    /// </summary>
    public interface IOrderRepositoryDto
    {
        /// <summary>
        /// Obtiene todas las ordenes del usuario.
        /// </summary>
        /// <returns></returns>
        Task<ServiceResponse<GetAllOrderDto>> GetAllOrderAsync();

        /// <summary>
        /// Confirma las ordenes de un usuario.
        /// </summary>
        /// <param name="confirmOrder"></param>
        /// <returns></returns>
        Task<ServiceResponse<bool>> ConfirmOrderAsync(ConfirmOrderDto confirmOrder);
    }
}
