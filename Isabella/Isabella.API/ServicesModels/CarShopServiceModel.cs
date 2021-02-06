namespace Isabella.API.ServicesModels
{
    using System.Threading.Tasks;

    using Data;
    using Models;
    using RepositorysModels;

    /// <summary>
    /// Servicio para la entidad que representa el carrito de compras.
    /// </summary>
    public class CarShopServiceModel : ICarShopRepositoryModel
    {
        private readonly DataContext _dataContext;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="dataContext"></param>
        public CarShopServiceModel(DataContext dataContext)
        {
            this._dataContext = dataContext;
        }


        /// <summary>
        /// Agrega un nuevo producto al carrito de compras de un usuario.
        /// </summary>
        /// <param name="carShop"></param>
        /// <returns></returns>
        public async Task AddProductsCarShopAsync(CarShop carShop)
        {
            await this._dataContext.CarShops.AddAsync(carShop).ConfigureAwait(false);
            await this._dataContext.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}
