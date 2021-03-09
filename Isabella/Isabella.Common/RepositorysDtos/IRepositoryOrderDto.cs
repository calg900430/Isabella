namespace Isabella.Common.RepositorysDtos
{
    using Isabella.Common.Dtos.Order;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Repositorio para el manejo de las ordenes(Pedidos)
    /// </summary>
    public interface IRepositoryOrderDto
    {
        /// <summary>
        /// Obtiene todas las ordenes del usuario.
        /// </summary>
        /// <param name="codeIdentification"></param>
        /// <returns></returns>
        Task<ServiceResponse<GetAllOrderDto>> GetAllOrderAsync(Guid codeIdentification);

        /// <summary>
        /// Confirma las ordenes de un usuario.
        /// </summary>
        /// <param name="confirmOrder"></param>
        /// <returns></returns>
        Task<ServiceResponse<bool>> ConfirmOrderAsync(ConfirmOrderDto confirmOrder);
    }
}
