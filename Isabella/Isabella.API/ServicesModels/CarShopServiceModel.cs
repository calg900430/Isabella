namespace Isabella.API.ServicesModels
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;

    using Data;
    using Models;
    using RepositorysModels;

    /// <summary>
    /// Servicio para la entidad que representa el carrito de compras.
    /// </summary>
    public class CarShopServiceModel : ICarShopRepositoryModel
    {
        private readonly DataContext _dataContext;
        private readonly IProductSpecialRepositoryModel _productSpecialRepositoryModel;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="dataContext"></param>
        /// <param name="productSpecialRepositoryModel"></param>
        public CarShopServiceModel(DataContext dataContext, IProductSpecialRepositoryModel productSpecialRepositoryModel)
        {
            this._dataContext = dataContext;
            this._productSpecialRepositoryModel = productSpecialRepositoryModel;
        }


        /// <summary>
        /// Agrega un nuevo producto al carrito de compras de un usuario.
        /// </summary>
        /// <param name="carShop"></param>
        /// <returns></returns>
        public async Task AddProductsCarShopAsync(CarShopProductStandard carShop)
        {
            await this._dataContext.CarShopsProductsStandards.AddAsync(carShop).ConfigureAwait(false);
            await this._dataContext.SaveChangesAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Agrega un nuevo producto al carrito de compras de un usuario.
        /// </summary>
        /// <param name="carShop"></param>
        /// <returns></returns>
        public async Task AddProductsCarShopAsync(CarShopProductSpecial carShop)
        {
            await this._dataContext.CarShopsProductsSpecials.AddAsync(carShop).ConfigureAwait(false);
            await this._dataContext.SaveChangesAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Devuelve el carrito de compras de productos de un usuario
        /// </summary>
        /// <param name="codeIdentification"></param>
        /// <returns></returns>
        public async Task<List<CarShopProductStandard>> GetMyCarShopProductStandard(CodeIdentification codeIdentification)
        => await this._dataContext.CarShopsProductsStandards
           .Include(c => c.ProductStandard)
           .ThenInclude(c => c.Category)
           .Where(c => c.CodeIdentification == codeIdentification)
           .ToListAsync();
          
        /// <summary>
        /// Devuelve el carrito de compras de productos de un usuario
        /// </summary>
        /// <param name="codeIdentification"></param>
        /// <returns></returns>
        public async Task<List<CarShopProductSpecial>> GetMyCarShopProductSpecial(CodeIdentification codeIdentification)
        {
            //Obtiene los pedidos de productos especiales
            var all_carshop_product_special = await this._dataContext.CarShopsProductsSpecials
            .Include(c => c.ProductSpecial)
            .ThenInclude(c => c.Category)
            .Where(c => c.CodeIdentification == codeIdentification)
            .ToListAsync();
            
            //Obtiene los productos de agregos del pedido.
            foreach(CarShopProductSpecial carShopProductSpecial in all_carshop_product_special)
            {
                var product_aggregate = await this._dataContext.CarShopProductAggregates
               .Include(c => c.ProductAggregate).ThenInclude(c => c.Category)
               .Where(c => c.CarShopProductSpecial == carShopProductSpecial)
               .ToListAsync();
               if(product_aggregate != null)
               carShopProductSpecial.CarShopProductAggregates = product_aggregate;
            }
            return all_carshop_product_special;
        }
    }
}
