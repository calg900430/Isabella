namespace Isabella.API.RepositorysModels
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Models;
    
    /// <summary>
    /// Repositorio para las categorias de los productos standards.
    /// </summary>
    public interface ICategoryRepositoryModel
    {
        /// <summary>
        /// Obtiene una categoria dado su nombre.
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public Task<Category> GetCategoryForNameAsync(string Name);

        /// <summary>
        /// Obtiene una categoria dado su Id.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public Task<Category> GetCategoryForIdAsync(int Id);

        /// <summary>
        /// Obtiene todas las categorias disponibles.
        /// </summary>
        /// <returns></returns>
        public Task<List<Category>> GetAllCategoryAsync();

        /// <summary>
        /// Agrega una nueva categoria.
        /// </summary>
        /// <param name="categoryProductStandard"></param>
        /// <returns></returns>
        public Task AddCategoryAsync(Category categoryProductStandard);
    }
}
