namespace Isabella.API.RepositorysModels
{
    using System.Threading.Tasks;
    using Isabella.API.Models;
    
    /// <summary>
    /// Repositorio para las categorias de los productos standards.
    /// </summary>
    public interface ICategoryProductAggregateRepositoryModel
    {
        /// <summary>
        /// Obtiene una categoria dado su Id.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public Task<CategoryProductAggregate> GetCategoryProductAggregateAsync(int Id);
    }
}
