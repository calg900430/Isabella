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
    /// Servicio para la entidad que representa las categorias de los productos de agregados.
    /// </summary>
    public class SubCategoryServiceModel : ISubCategoryRepositoryModel
    {
        private readonly DataContext _dataContext;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataContext"></param>
        public SubCategoryServiceModel(DataContext dataContext)
        {
            this._dataContext = dataContext;
        }

        /// <summary>
        /// Agrega una nueva categoria.
        /// </summary>
        /// <param name="subCategory"></param>
        /// <returns></returns>
        public async Task AddSubCategoryAsync(SubCategory subCategory)
        {
           await this._dataContext.SubCategories
           .AddAsync(subCategory)
           .ConfigureAwait(false);
           await this._dataContext.SaveChangesAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Obtiene todas las categorias dispnibles.
        /// </summary>
        /// <returns></returns>
        public async Task<List<SubCategory>> GetAllSubCategoryAsync()
        => await this._dataContext.SubCategories
        .ToListAsync()
        .ConfigureAwait(false);

        /// <summary>
        /// Obtiene una categoria dado su Id.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<SubCategory> GetSubCategoryProductForIdAsync(int Id)
        => await this._dataContext.SubCategories
        .FirstOrDefaultAsync(c => c.Id == Id)
        .ConfigureAwait(false);

        /// <summary>
        /// Obtiene una categoria dado su nombre.
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public async Task<SubCategory> GetSubCategoryForNameAsync(string Name)
        => await this._dataContext.SubCategories
        .FirstOrDefaultAsync(c => c.Name == Name)
        .ConfigureAwait(false);
    }
}
