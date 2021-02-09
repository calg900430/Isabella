namespace Isabella.API.RepositorysModels
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Models;
    
    /// <summary>
    /// Repositorio para las subcategorias.
    /// </summary>
    public interface ISubCategoryRepositoryModel
    {
        /// <summary>
        /// Obtiene una categoria dado su nombre.
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public Task<SubCategory> GetSubCategoryForNameAsync(string Name);

        /// <summary>
        /// Obtiene una categoria dado su Id.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public Task<SubCategory> GetSubCategoryProductForIdAsync(int Id);

        /// <summary>
        /// Obtiene todas las categorias disponibles.
        /// </summary>
        /// <returns></returns>
        public Task<List<SubCategory>> GetAllSubCategoryAsync();

        /// <summary>
        /// Agrega una nueva categoria.
        /// </summary>
        /// <param name="subCategory"></param>
        /// <returns></returns>
        public Task AddSubCategoryAsync(SubCategory subCategory);
    }
}
