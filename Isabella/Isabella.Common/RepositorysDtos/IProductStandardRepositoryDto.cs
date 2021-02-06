namespace Isabella.Common.RepositorysDtos
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Common;
    using Common.Dtos.ProductStandard;

    /// <summary>
    /// Repositorio para el manejo de los Dtos de los productos standard.
    /// </summary>
    public interface IProductStandardRepositoryDto
    {
        /// <summary>
        /// Agrega un nuevo producto.
        /// </summary>
        /// <param name="addProductStandard"></param>
        /// <returns></returns>
        Task<ServiceResponse<bool>> AddProductStandardAsync(AddProductStandardDto addProductStandard);

        /// <summary>
        /// Actualiza un producto.
        /// </summary>
        /// <param name="updateProductStandard"></param>
        /// <returns></returns>
        Task<ServiceResponse<GetProductStandardDto>> UpdateProductStandardAsync(UpdateProductStandardDto updateProductStandard);

        /// <summary>
        /// Obtiene un producto dado su Id.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        Task<ServiceResponse<GetProductStandardDto>> GetProductStandardForIdAsync(int Id);

        /// <summary>
        /// Obtiene todos los productos disponibles en la base de datos.
        /// </summary>
        /// <returns></returns>
        Task<ServiceResponse<List<GetProductStandardDto>>> GetAllProductStandardAsync();

        /// <summary>
        /// Obtiene el Id del último producto.
        /// </summary>
        /// <returns></returns>
        Task<ServiceResponse<int>> GetIdOfLastProductStandardAsync();

        /// <summary>
        /// Obtiene la imagen de un producto.
        /// </summary>
        /// <returns></returns>
        Task<ServiceResponse<GetImageProductStandardDto>> GetImageProductStandardForIdAsync(int ProductStandardId, int ImageId);

        /// <summary>
        /// Agrega una imagen a un producto.
        /// </summary>
        /// <param name="addImageProductStandard"></param>
        /// <returns></returns>
        Task<ServiceResponse<bool>> AddImageProductStandardAsync(AddImageProductStandardDto addImageProductStandard);

        /// <summary>
        /// Borra una imagen de un producto.
        /// </summary>
        /// <param name="ProductId"></param>
        /// <param name="ImageId"></param>
        /// <returns></returns>
        Task<ServiceResponse<bool>> DeleteImageProductStandardAsync(int ProductId, int ImageId);

        /// <summary>
        /// Pone un producto en disponible.
        /// </summary>
        /// <param name="ProductId"></param>
        /// <returns></returns>
        Task<ServiceResponse<bool>> EnableProductStandardAsync(int ProductId);

        /// <summary>
        /// Pone un producto en no disponible.
        /// </summary>
        /// <param name="ProductId"></param>
        /// <returns></returns>
        Task<ServiceResponse<bool>> DisableProductStandardAsync(int ProductId);

        /// <summary>
        /// Obtiene una cantidad determinada de productos dado un producto de referencia y la cantidad.
        /// </summary>
        /// <param name="ProductId"></param>
        /// <param name="CantProduct"></param>
        /// <returns></returns>
        Task<ServiceResponse<List<GetProductStandardDto>>> GetCantProductStandardAsync(int ProductId, int CantProduct);

        /// <summary>
        /// Devuelve una lista con los Id de todas las imagenes del producto.
        /// </summary>
        /// <param name="ProductId"></param>
        /// <returns></returns>
        Task<ServiceResponse<List<int>>> GetListIdOfImageProductStandardAsync(int ProductId);

        /// <summary>
        /// Devuelve todas las imagenes de un producto.
        /// </summary>
        /// <param name="ProductId"></param>
        /// <returns></returns>
        Task<ServiceResponse<List<GetImageProductStandardDto>>> GetAllImageProductStandardAsync(int ProductId);

        /// <summary>
        /// Devuelve una cantidad de imagenes de un producto a partir de una imagen de referencia.
        /// </summary>
        /// <param name="ProductId"></param>
        /// <param name="Image"></param>
        /// <param name="CantImage"></param>
        /// <returns></returns>
        Task<ServiceResponse<List<GetImageProductStandardDto>>> GetCantImageProductStandardAsync(int ProductId, int Image, int CantImage);

    }
}
