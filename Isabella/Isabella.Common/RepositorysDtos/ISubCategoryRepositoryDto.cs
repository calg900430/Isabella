namespace Isabella.Common.RepositorysDtos
{
    using Dtos.SubCategorie;
    using Isabella.Common.Dtos.Categorie;
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
        Task<ServiceResponse<List<GetSubCategorieDto>>> GetAllSubCategoryAsync();

        /// <summary>
        /// Obtiene todas las subcategorias que están disponibles.
        /// </summary>
        Task<ServiceResponse<List<GetSubCategorieDto>>> GetAllSubCategoryIsAvailableAsync();

        /// <summary>
        /// Obtiene todas las subcategorias que no están disponibles.
        /// </summary>
        Task<ServiceResponse<List<GetSubCategorieDto>>> GetAllSubCategoryIsNotAvailableAsync();

        /// <summary>
        /// Obtiene una subcategoria por su nombre.
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        Task<ServiceResponse<GetSubCategorieDto>> GetSubCategoryForNameAsync(string Name);

        /// <summary>
        /// Obtiene una subcategoria de un producto por su Id.
        /// </summary>
        /// <param name="SubCategoryId"></param>
        /// <returns></returns>
        Task<ServiceResponse<GetSubCategorieDto>> GetSubCategoryForIdAsync(int SubCategoryId);

        /// <summary>
        /// Agrega una nueva subcategoria a un producto.
        /// </summary>
        /// <param name="addSubCategoryProduct"></param>
        /// <returns></returns>
        Task<ServiceResponse<bool>> AddSubCategoryAsync(AddSubCategorieToProductDto addSubCategoryProduct);

        /// <summary>
        /// Actualiza una nueva subcategoria.
        /// </summary>
        /// <param name="updateSubCategoryDto"></param>
        /// <returns></returns>
        Task<ServiceResponse<bool>> UpdateSubCategoryAsync(UpdateSubCategorieDto updateSubCategoryDto);

        /// <summary>
        /// Elimina una subcategoria.
        /// </summary>
        /// <param name="SubCategoryId"></param>
        /// <returns></returns>
        Task<ServiceResponse<bool>> DeleteSubCategoryAsync(int SubCategoryId);
    }
}
