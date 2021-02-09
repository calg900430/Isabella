namespace Isabella.API.ServicesModels
{
    using System.Threading.Tasks;
    using System.Linq;
    using Microsoft.EntityFrameworkCore;

    using Models;
    using RepositorysModels;
    using Data;
    using System.Collections.Generic;

    /// <summary>
    /// Servicio para la entidad que representa las categorias de los productos standards.
    /// </summary>
    public class CategoryServiceModel : ICategoryRepositoryModel
    {
        private readonly DataContext _dataContext;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataContext"></param>
        public CategoryServiceModel(DataContext dataContext)
        {
            this._dataContext = dataContext;
        }

        /// <summary>
        /// Agrega una nueva categoria.
        /// </summary>
        /// <param name="categoryProductStandard"></param>
        /// <returns></returns>
        public async Task AddCategoryAsync(Category categoryProductStandard)
        {
           await this._dataContext.Categories
          .AddAsync(categoryProductStandard)
          .ConfigureAwait(false);
           await this._dataContext.SaveChangesAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Obtiene todas las categorias disponibles.
        /// </summary>
        /// <returns></returns>
        public async Task<List<Category>> GetAllCategoryAsync()
         => await this._dataContext.Categories
        .ToListAsync()
        .ConfigureAwait(false);

        /// <summary>
        /// Obtiene una categoria dado su Id.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<Category> GetCategoryForIdAsync(int Id)
         => await this._dataContext.Categories
        .FirstOrDefaultAsync(c => c.Id == Id)
        .ConfigureAwait(false);


        /// <summary>
        /// Obtiene una categoria dado su nombre.
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public async Task<Category> GetCategoryForNameAsync(string Name)
        => await this._dataContext.Categories
        .FirstOrDefaultAsync(c => c.Name == Name)
        .ConfigureAwait(false);
    }
}
