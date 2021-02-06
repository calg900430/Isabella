namespace Isabella.Web.Repositorys.API
{
    using System.Linq;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Common;
    using Common.Dtos.Order;
    using Services;
    using Models.Entities;

    /// <summary>
    /// Servicio para gestionar la entidad Order(Pedidos)
    /// </summary>
    public interface IOrderRepositoryAPI: IGenericRepository <Order>
    {
        
        /// <summary>
        /// Accede al carrito de compras del usuario.
        /// </summary>
        /// <returns></returns>
        Task<ServiceResponse<List<GetCarShopDto>>> GetCarsShopAsync(string UserName);

        /// <summary>
        /// Agrega un producto al carrito de compras.
        /// </summary>
        /// <param name="updateCarShop"></param>
        /// <returns></returns>
        Task<ServiceResponse<bool>> UpdateCarShopAsync(UpdateCarShopDto updateCarShop);

        /// <summary>
        /// Agrega un producto al carrito de compras.
        /// </summary>
        /// <param name="addProductToCarShopDto"></param>
        /// <returns></returns>
        Task<ServiceResponse<bool>> AddProductToCarShopAsync(AddProductToCarShopDto addProductToCarShopDto);

        /// <summary>
        /// Agrega un producto Pizza o Pasta al carrito de compras.
        /// </summary>
        /// <param name="addProductToCarShopDto"></param>
        /// <returns></returns>
        Task<ServiceResponse<bool>> AddProduct_PizzasPastas_ToCarShopAsync(AddProduct_PizzaPasta_ToCarShopDto addProductToCarShopDto);


        /// <summary>
        /// Elimina un Producto de su carrito de compras
        /// </summary>
        /// <param name="deleteProductToCarShop"></param>
        /// <returns></returns>
        Task<ServiceResponse<bool>> DeleteProductToCarShopAsync(DelProductToCarShopDto deleteProductToCarShop);

        /// <summary>
        /// Realiza el pedido del usuario.
        /// </summary>
        /// <param name="confirmOrderDto"></param>
        /// <returns></returns>
        Task<ServiceResponse<bool>> ConfirmAllOrdersOfUserAsync(AddConfirmOrderDto confirmOrderDto);

        /// <summary>
        /// Obtiene todos los actuales pedidos del usuario.
        /// </summary>
        /// <param name="UserName"></param>
        /// <returns></returns>
        Task<ServiceResponse<List<GetAllOrdersOfUserDto>>> GetAllOrderOfUserAsync(string UserName);
    }
}
