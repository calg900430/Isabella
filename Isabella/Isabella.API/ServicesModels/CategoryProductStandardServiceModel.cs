namespace Isabella.API.ServicesModels
{
    using System.Threading.Tasks;
    using System.Linq;
    using Microsoft.EntityFrameworkCore;

    using Models;
    using RepositorysModels;
    using Data;

    /// <summary>
    /// Servicio para la entidad que representa las categorias de los productos standards.
    /// </summary>
    public class CategoryProductStandardServiceModel : ICategoryProductStandardRepositoryModel
    {
        private readonly DataContext _dataContext;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataContext"></param>
        public CategoryProductStandardServiceModel(DataContext dataContext)
        {
            this._dataContext = dataContext;
        }

        /// <summary>
        /// Obtiene una categoria standard dado su Id.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<CategoryProductStandard> GetCategoryProductStandardAsync(int Id)
        => await this._dataContext.CategoryProductStandards
          .FirstOrDefaultAsync(c => c.Id == Id)
          .ConfigureAwait(false);
    }
}
