namespace Isabella.Common.RepositorysDtos
{
    using System;
    using System.Threading.Tasks;
    using Dtos.CarShop;

    /// <summary>
    /// Repositorio para el manejo de los Dtos del carrito de compras.
    /// </summary>
    public interface ICarShopDto
    {
        /// <summary>
        /// Agrega productos al carrito de compras.
        /// </summary>
        /// <param name="addProductsToCarShop"></param>
        /// <returns></returns>
        Task<ServiceResponse<bool>> AddProductsToCarShop(AddProductToCarShopDto addProductsToCarShop);

     
        /// <summary>
        /// Obtiene el carrito de compras para la posible compra de un usuario
        /// </summary>
        /// <param name="CodeVerification"></param>
        /// <returns></returns>
        Task<ServiceResponse<GetAllProductOfMyCarShopDto>> GetMyCarShop(Guid CodeVerification);
    }
}
