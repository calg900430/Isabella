namespace Isabella.Common.RepositorysDtos
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Common;
    using Common.Dtos.ProductSpecial;

    /// <summary>
    /// Repositorio para el manejo de los Dtos de los productos especiales.
    /// </summary>
    public interface IProductSpecialRepositoryDto
    {
        /// <summary>
        /// Agrega un nuevo producto.
        /// </summary>
        /// <param name="addProductSpecial"></param>
        /// <returns></returns>
        Task<ServiceResponse<bool>> AddProductSpecialAsync(AddProductStandardSpecialDto addProductSpecial);

        /// <summary>
        /// Actualiza un producto.
        /// </summary>
        /// <param name="updateProductSpecial"></param>
        /// <returns></returns>
        Task<ServiceResponse<GetProductSpecialDto>> UpdateProductSpecialAsync(UpdateProductSpecialDto updateProductSpecial);

        /// <summary>
        /// Obtiene un producto dado su Id.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        Task<ServiceResponse<GetProductSpecialDto>> GetProductSpecialForIdAsync(int Id);

        /// <summary>
        /// Obtiene todos los productos disponibles en la base de datos.
        /// </summary>
        /// <returns></returns>
        Task<ServiceResponse<List<GetProductSpecialDto>>> GetAllProductSpecialAsync();

        /// <summary>
        /// Obtiene el Id del último producto.
        /// </summary>
        /// <returns></returns>
        Task<ServiceResponse<int>> GetIdOfLastProductSpecialAsync();

        /// <summary>
        /// Obtiene la imagen de un producto dado el Id del producto y la imagen
        /// </summary>
        /// <param name="ProductStandardId"></param>
        /// <param name="ImageId"></param>
        /// <returns></returns>
        Task<ServiceResponse<GetImageProductSpecialDto>> GetImageProductSpecialForIdAsync(int ProductStandardId, int ImageId);

        /// <summary>
        /// Agrega una imagen a un producto.
        /// </summary>
        /// <param name="addImageProductSpecial"></param>
        /// <returns></returns>
        Task<ServiceResponse<bool>> AddImageProductSpecialAsync(AddImageProductSpecialDto addImageProductSpecial);

        /// <summary>
        /// Borra una imagen de un producto.
        /// </summary>
        /// <param name="ProductId"></param>
        /// <param name="ImageId"></param>
        /// <returns></returns>
        Task<ServiceResponse<bool>> DeleteImageProductSpecialAsync(int ProductId, int ImageId);

        /// <summary>
        /// Pone un producto en disponible.
        /// </summary>
        /// <param name="ProductId"></param>
        /// <returns></returns>
        Task<ServiceResponse<bool>> EnableProductSpecialAsync(int ProductId);

        /// <summary>
        /// Pone un producto en no disponible.
        /// </summary>
        /// <param name="ProductId"></param>
        /// <returns></returns>
        Task<ServiceResponse<bool>> DisableProductSpecialAsync(int ProductId);

        /// <summary>
        /// Obtiene una cantidad determinada de productos dado un producto de referencia y la cantidad.
        /// </summary>
        /// <param name="ProductId"></param>
        /// <param name="CantProduct"></param>
        /// <returns></returns>
        Task<ServiceResponse<List<GetProductSpecialDto>>> GetCantProductSpecialAsync(int ProductId, int CantProduct);

        /// <summary>
        /// Devuelve una lista con los Id de todas las imagenes del producto.
        /// </summary>
        /// <param name="ProductId"></param>
        /// <returns></returns>
        Task<ServiceResponse<List<int>>> GetListIdOfImageProductSpecialAsync(int ProductId);

        /// <summary>
        /// Devuelve todas las imagenes de un producto.
        /// </summary>
        /// <param name="ProductId"></param>
        /// <returns></returns>
        Task<ServiceResponse<List<GetImageProductSpecialDto>>> GetAllImageProductSpecialAsync(int ProductId);

        /// <summary>
        /// Devuelve una cantidad de imagenes de un producto a partir de una imagen de referencia.
        /// </summary>
        /// <param name="ProductId"></param>
        /// <param name="Image"></param>
        /// <param name="CantImage"></param>
        /// <returns></returns>
        Task<ServiceResponse<List<GetImageProductSpecialDto>>> GetCantImageProductSpecialAsync(int ProductId, int Image, int CantImage);

    }
}
