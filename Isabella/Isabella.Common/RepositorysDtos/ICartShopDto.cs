namespace Isabella.Common.RepositorysDtos
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Dtos.CarShop;

    /// <summary>
    /// Repositorio para el manejo de los Dtos del carrito de compras.
    /// </summary>
    public interface ICartShopDto
    {
        /// <summary>
        /// Agrega productos al carrito de compras.
        /// </summary>
        /// <param name="addProductsToCarShop"></param>
        /// <returns></returns>
        Task<ServiceResponse<GetCarShopProductDto>> AddProductsToCartShopAsync(AddProductToCartShopDto addProductsToCarShop);

        /// <summary>
        /// Obtiene el carrito de compras para la posible compra de un usuario
        /// </summary>
        /// <returns></returns>
        Task<ServiceResponse<GetAllProductOfCartShopDto>> GetMyCartShopAsync();

        /// <summary>
        /// Actualiza la cantidad de un producto.
        /// </summary>
        /// <param name="modifyQuantityProduct"></param>
        /// <returns></returns>
        Task<ServiceResponse<bool>> UpdateQuantityProductAsync(ModifyQuantityProductDto modifyQuantityProduct);

        /// <summary>
        /// Agrega un nuevo agregado a un producto que está en el carrito de compras.
        /// </summary>
        /// <param name="addAggregateInProduct"></param>
        /// <returns></returns>
        Task<ServiceResponse<bool>> AddAggregateInProductAsync(AddAggregateInProductDto addAggregateInProduct);

        /// <summary>
        /// Actualiza la cantidad de un agregado.
        /// </summary>
        /// <param name="updateAggregateProduct"></param>
        /// <returns></returns>
        Task<ServiceResponse<bool>> UpdateQuantityInAggregateProductAsync(ModifyCantInAggregateProductDto updateAggregateProduct);

        /// <summary>
        /// Incrementa la cantidad de un agregado en un producto en un valor dado.
        /// </summary>
        /// <param name="updateAggregateProduct"></param>
        /// <returns></returns>
        Task<ServiceResponse<bool>> IncrementQuantityInAggregateProductAsync(ModifyCantInAggregateProductDto updateAggregateProduct);

        /// <summary>
        /// Incrementa la cantidad de un producto en un valor dado.
        /// </summary>
        /// <param name="modifyQuantityProduct"></param>
        /// <returns></returns>
        Task<ServiceResponse<bool>> IncrementQuantityProductAsync(ModifyQuantityProductDto modifyQuantityProduct);

        /// <summary>
        /// Elimina un producto del carrito de compras
        /// </summary>
        /// <param name="ProductCombinedId"></param>
        /// <returns></returns>
        Task<ServiceResponse<bool>> RemoveProductOfCartShopAsync(int ProductCombinedId);

        /// <summary>
        /// Elimina un agregado de un producto del carrito de compras.
        /// </summary>
        /// <param name="ProductCombinedId"></param>
        /// <param name="AggregateId"></param>
        /// <returns></returns>
        Task<ServiceResponse<bool>> RemoveAggregateInProductOfCartShopAsync(int ProductCombinedId, int AggregateId);

        /// <summary>
        /// Actualiza la subcategoria de un producto.
        /// </summary>
        /// <param name="updateSubCategoryProduct"></param>
        /// <returns></returns>
        Task<ServiceResponse<bool>> UpdateSubCategoryAsync(UpdateSubCategoryProductDto updateSubCategoryProduct);

        /// <summary>
        /// Elimina la subcategoria de un producto que está en el carrito.
        /// </summary>
        /// <param name="updateSubCategoryProduct"></param>
        /// <returns></returns>
        Task<ServiceResponse<bool>> RemoveSubCategoryAsync(int ProductCombinedId);

        /// <summary>
        /// Elimina todos los productos del carrito de compras del usuario.
        /// </summary>
        /// <param name="CodeIdentifaction"></param>
        /// <returns></returns>
        Task<ServiceResponse<bool>> RemoveAllCarShopAsync();

        /// <summary>
        /// Actualiza la carrito del usuario.
        /// </summary>
        /// <param name="updateCartShop"></param>
        /// <returns></returns>
        Task<ServiceResponse<bool>> UpdateCartShopAsync(UpdateCartShopDto updateCartShop);

    }
}
