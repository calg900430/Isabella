namespace Isabella.API.RepositorysModels
{
    using Isabella.API.Models;
    using System.Threading.Tasks;

    /// <summary>
    /// Repositorio para el carrito de compras.
    /// </summary>
    public interface ICarShopRepositoryModel
    {
        /// <summary>
        /// Agrega un nuevo pedido al carrito de compras de un usuario informal.
        /// </summary>
        /// <param name="carShop"></param>
        /// <returns></returns>
        public Task AddProductsCarShopAsync(CarShop carShop);
    }
}
