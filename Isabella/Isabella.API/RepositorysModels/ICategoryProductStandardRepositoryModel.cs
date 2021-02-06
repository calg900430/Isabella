namespace Isabella.API.RepositorysModels
{
    using System.Threading.Tasks;
    using Isabella.API.Models;
    
    /// <summary>
    /// Repositorio para las categorias de los productos standards.
    /// </summary>
    public interface ICategoryProductStandardRepositoryModel
    {
        /// <summary>
        /// Obtiene una categoria standard dado su Id.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public Task<CategoryProductStandard> GetCategoryProductStandardAsync(int Id);
    }
}
