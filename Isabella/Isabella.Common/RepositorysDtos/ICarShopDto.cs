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
        Task<ServiceResponse<bool>> AddProductsToCarShop(AddProductStandardToCarShopDto addProductsToCarShop);

        /// <summary>
        /// Agrega productos al carrito de compras.
        /// </summary>
        /// <param name="addProductsToCarShop"></param>
        /// <returns></returns>
        Task<ServiceResponse<bool>> AddProductsToCarShop(AddProductSpecialToCarShopDto addProductsToCarShop);

        /// <summary>
        /// Obtiene el caarito de compras para la posible compra de un usuario
        /// </summary>
        /// <param name="CodeVerification"></param>
        /// <returns></returns>
        Task<ServiceResponse<GetCarShopDto>> GetMyCarShop(Guid CodeVerification);
    }
}
