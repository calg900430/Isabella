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
        /// Obtiene todas las subcategorias.
        /// </summary>
        Task<ServiceResponse<List<GetSubCategoryDto>>> GetAllSubCategoryAsync();

        /// <summary>
        /// Obtiene todas las subcategorias que están disponibles.
        /// </summary>
        Task<ServiceResponse<List<GetSubCategoryDto>>> GetAllSubCategoryIsAvailableAsync();

        /// <summary>
        /// Obtiene todas las subcategorias que no están disponibles.
        /// </summary>
        Task<ServiceResponse<List<GetSubCategoryDto>>> GetAllSubCategoryIsNotAvailableAsync();

        /// <summary>
        /// Obtiene una subcategoria por su nombre.
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        Task<ServiceResponse<GetSubCategoryDto>> GetSubCategoryForNameAsync(string Name);

        /// <summary>
        /// Obtiene una subcategoria de un producto por su Id.
        /// </summary>
        /// <param name="SubCategoryId"></param>
        /// <returns></returns>
        Task<ServiceResponse<GetSubCategoryDto>> GetSubCategoryForIdAsync(int SubCategoryId);

        /// <summary>
        /// Agrega una nueva subcategoria a un producto.
        /// </summary>
        /// <param name="addSubCategoryProduct"></param>
        /// <returns></returns>
        Task<ServiceResponse<bool>> AddSubCategoryAsync(AddSubCategoryToProductDto addSubCategoryProduct);

        /// <summary>
        /// Actualiza una nueva subcategoria.
        /// </summary>
        /// <param name="updateSubCategoryDto"></param>
        /// <returns></returns>
        Task<ServiceResponse<bool>> UpdateSubCategoryAsync(UpdateSubCategoryDto updateSubCategoryDto);

        /// <summary>
        /// Elimina una subcategoria.
        /// </summary>
        /// <param name="SubCategoryId"></param>
        /// <returns></returns>
        Task<ServiceResponse<bool>> DeleteSubCategoryAsync(int SubCategoryId);
    }
}
