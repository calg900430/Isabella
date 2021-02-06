namespace Isabella.API.ServicesModels
{
    using System.Threading.Tasks;
    using System.Linq;
    using Microsoft.EntityFrameworkCore;

    using Models;
    using RepositorysModels;
    using Data;

    /// <summary>
    /// Servicio para la entidad que representa las categorias de los productos de agregados.
    /// </summary>
    public class CategoryProductAggregateServiceModel : ICategoryProductAggregateRepositoryModel
    {
        private readonly DataContext _dataContext;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataContext"></param>
        public CategoryProductAggregateServiceModel(DataContext dataContext)
        {
            this._dataContext = dataContext;
        }

        /// <summary>
        /// Obtiene una categoria special dado su Id.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<CategoryProductAggregate> GetCategoryProductAggregateAsync(int Id)
        => await this._dataContext.CategoryProductAggregates
        .FirstOrDefaultAsync(c => c.Id == Id)
        .ConfigureAwait(false);
    }
}
