namespace Isabella.API.RepositorysModels
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Models;

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
        public Task AddProductsCarShopAsync(CarShopProductStandard carShop);

        /// <summary>
        /// Agrega un nuevo pedido al carrito de compras de un usuario informal.
        /// </summary>
        /// <param name="carShop"></param>
        /// <returns></returns>
        public Task AddProductsCarShopAsync(CarShopProductSpecial carShop);

        /// <summary>
        /// Devuelve el carrito de compras de un usuario.
        /// </summary>
        /// <param name="codeIdentification"></param>
        /// <returns></returns>
        public Task<List<CarShopProductStandard>> GetMyCarShopProductStandard(CodeIdentification codeIdentification);

        /// <summary>
        /// Devuelve el carrito de compras de un usuario.
        /// </summary>
        /// <param name="codeIdentification"></param>
        /// <returns></returns>
        public Task<List<CarShopProductSpecial>> GetMyCarShopProductSpecial(CodeIdentification codeIdentification);
    }
}
