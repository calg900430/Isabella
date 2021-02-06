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
    public class CategoryProductSpecialServiceModel : ICategoryProductSpecialRepositoryModel
    {
        private readonly DataContext _dataContext;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataContext"></param>
        public CategoryProductSpecialServiceModel(DataContext dataContext)
        {
            this._dataContext = dataContext;
        }

        /// <summary>
        /// Obtiene una categoria special dado su Id.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<CategoryProductSpecial> GetCategoryProductSpecialAsync(int Id)
        => await this._dataContext.CategoryProductSpecials
          .FirstOrDefaultAsync(c => c.Id == Id)
          .ConfigureAwait(false);
    }
}
