namespace Isabella.Common.RepositorysDtos
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Common;
    using Common.Dtos.Product;

    /// <summary>
    /// Repositorio para el manejo de los Dtos de los productos standard.
    /// </summary>
    public interface IProductRepositoryDto
    {
        /// <summary>
        /// Agrega un nuevo producto.
        /// </summary>
        /// <param name="addProduct"></param>
        /// <returns></returns>
        Task<ServiceResponse<bool>> AddProductAsync(AddProductDto addProduct);

        /// <summary>
        /// Actualiza un producto.
        /// </summary>
        /// <param name="updateProduct"></param>
        /// <returns></returns>
        Task<ServiceResponse<GetProductDto>> UpdateProductAsync(UpdateProductDto updateProduct);

        /// <summary>
        /// Obtiene un producto dado su Id.
        /// </summary>
        /// <param name="ProductId"></param>
        /// <returns></returns>
        Task<ServiceResponse<GetProductDto>> GetProductForIdAsync(int ProductId);

        /// <summary>
        /// Obtiene todos los productos disponibles en la base de datos.
        /// </summary>
        /// <returns></returns>
        Task<ServiceResponse<List<GetProductDto>>> GetAllProductAsync();

        /// <summary>
        /// Obtiene el Id del último producto.
        /// </summary>
        /// <returns></returns>
        Task<ServiceResponse<int>> GetIdOfLastProductAsync();

        /// <summary>
        /// Obtiene la imagen de un producto.
        /// </summary>
        /// <returns></returns>
        Task<ServiceResponse<GetImageProductDto>> GetImageProductForIdAsync(int ProductId, int ImageId);

        /// <summary>
        /// Obtiene la primera imagen de un producto.
        /// </summary>
        /// <returns></returns>
        Task<ServiceResponse<GetImageProductDto>> GetFirstImageProductAsync(int ProductId);

        /// <summary>
        /// Agrega una imagen a un producto.
        /// </summary>
        /// <param name="addImageProduct"></param>
        /// <returns></returns>
        Task<ServiceResponse<bool>> AddImageProductAsync(AddImageProductDto addImageProduct);

        /// <summary>
        /// Borra una imagen de un producto.
        /// </summary>
        /// <param name="ProductId"></param>
        /// <param name="ImageId"></param>
        /// <returns></returns>
        Task<ServiceResponse<bool>> DeleteImageProductAsync(int ProductId, int ImageId);

        /// <summary>
        /// Establece un producto en disponible o no disponible.
        /// </summary>
        /// <param name="ProductId"></param>
        /// <param name="enable"></param>
        /// <returns></returns>
        Task<ServiceResponse<bool>> EnableProductAsync(int ProductId, bool enable);

        /// <summary>
        /// Obtiene una cantidad determinada de productos dado un producto de referencia y la cantidad.
        /// </summary>
        /// <param name="ProductId"></param>
        /// <param name="cantProduct"></param>
        /// <returns></returns>
        Task<ServiceResponse<List<GetProductDto>>> GetCantProductAsync(int ProductId, int cantProduct);

        /// <summary>
        /// Devuelve una lista con los Id de todas las imagenes del producto.
        /// </summary>
        /// <param name="ProductId"></param>
        /// <returns></returns>
        Task<ServiceResponse<List<int>>> GetListIdOfImageProductAsync(int ProductId);

        /// <summary>
        /// Devuelve todas las imagenes de un producto.
        /// </summary>
        /// <param name="ProductId"></param>
        /// <returns></returns>
        Task<ServiceResponse<List<GetImageProductDto>>> GetAllImageProductAsync(int ProductId);

        /// <summary>
        /// Devuelve una cantidad de imagenes de un producto a partir de una imagen de referencia.
        /// </summary>
        /// <param name="ProductId"></param>
        /// <param name="Image"></param>
        /// <param name="cantImage"></param>
        /// <returns></returns>
        Task<ServiceResponse<List<GetImageProductDto>>> GetCantImageProductAsync(int ProductId, int Image, int cantImage);

        /// <summary>
        /// Borra un producto.
        /// </summary>
        /// <param name="ProductId"></param>
        /// <returns></returns>
        Task<ServiceResponse<bool>> DeleteProductAsync(int ProductId);

        /// <summary>
        /// Obtiene un producto dado su Id si el mismo está disponible.
        /// </summary>
        /// <param name="ProductId"></param>
        /// <returns></returns>
        Task<ServiceResponse<GetProductDto>> GetProductIsAvailableForIdAsync(int ProductId);

        /// <summary>
        /// Obtiene todos los productos disponibles en la base de datos.
        /// </summary>
        /// <returns></returns>
        Task<ServiceResponse<List<GetProductDto>>> GetAllProductIsAvailableAsync();

        /// <summary>
        /// Obtiene una cantidad determinada de productos dado un producto de referencia y la cantidad.
        /// </summary>
        /// <param name="ProductId"></param>
        /// <param name="cantProduct"></param>
        /// <returns></returns>
        Task<ServiceResponse<List<GetProductDto>>> GetCantProductIsAvailableAsync(int ProductId, int cantProduct);

        /// <summary>
        /// Obtiene todos los productos de una categoria determinada.
        /// </summary>
        /// <param name="CategoryId"></param>
        /// <returns></returns>
        Task<ServiceResponse<List<GetProductDto>>> GetAllProductOfCategory(int CategoryId);

        /// <summary>
        /// Obtiene todos los productos disponibles de una categoria determinada.
        /// </summary>
        /// <param name="CategoryId"></param>
        /// <returns></returns>
        Task<ServiceResponse<List<GetProductDto>>> GetAllProductIsAvailableOfCategory(int CategoryId);
    }
}
