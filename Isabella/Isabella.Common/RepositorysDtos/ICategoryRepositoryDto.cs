namespace Isabella.Common.RepositorysDtos
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Dtos.Category;

    /// <summary>
    /// Repositorio de las categorias.
    /// </summary>
    public interface ICategoryRepositoryDto
    {
        /// <summary>
        /// Obtiene todas las categorias disponibles para los productos.
        /// </summary>
        Task<ServiceResponse<List<GetCategoryDto>>> GetAllCategoryAsync();

        /// <summary>
        /// Obtiene una categoria de un producto por su nombre.
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        Task<ServiceResponse<GetCategoryDto>> GetCategoryForNameAsync(string Name);

        /// <summary>
        /// Obtiene una categoria de un producto por su Id.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        Task<ServiceResponse<GetCategoryDto>> GetCategoryForIdAsync(int Id);

        /// <summary>
        /// Agrega una nueva categoria para un producto.
        /// </summary>
        /// <param name="addCategory"></param>
        /// <returns></returns>
        Task<ServiceResponse<bool>> AddCategoryAsync(AddCategoryDto addCategory);

        /// <summary>
        /// Borra una categoria.
        /// </summary>
        /// <param name="CategoryId"></param>
        /// <returns></returns>
        Task<ServiceResponse<bool>> DeleteCategoryAsync(int CategoryId);

        /// <summary>
        /// Devuelve las categorias de los productos que esten disponibles. 
        /// </summary>
        /// <returns></returns>
        Task<ServiceResponse<List<GetCategoryDto>>> GetAllCategoryIsProductIsAvailableAsync();

        /// <summary>
        ///  Actualiza una categoria.
        /// </summary>
        /// <param name="updateCategory"></param>
        /// <returns></returns>
        Task<ServiceResponse<bool>> UpdateCategoryAsync(UpdateCategoryDto updateCategory);
    }
}
