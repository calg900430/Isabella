namespace Isabella.Common.RepositorysDtos
{
    using Dtos.SubCategory;
    using Isabella.Common.Dtos.Category;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Repositorio de las subcategorias.
    /// </summary>
    public interface ISubCategoryRepositoryDto
    {
        /// <summary>
        /// Obtiene todas las subcategorias disponibles.
        /// </summary>
        Task<ServiceResponse<List<GetSubCategoryDto>>> GetAllSubCategoryAsync();

        /// <summary>
        /// Obtiene una subcategoria por su nombre.
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        Task<ServiceResponse<GetSubCategoryDto>> GetSubCategoryForNameAsync(string Name);

        /// <summary>
        /// Obtiene una categoria de un producto por su Id.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        Task<ServiceResponse<GetSubCategoryDto>> GetSubCategoryForIdAsync(int Id);

        /// <summary>
        /// Agrega una nueva subcategoria.
        /// </summary>
        /// <param name="addSubCategoryProduct"></param>
        /// <returns></returns>
        Task<ServiceResponse<bool>> AddSubCategoryAsync(AddSubCategoryDto addSubCategoryProduct);
    }
}
