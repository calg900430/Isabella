namespace Isabella.API.RepositorysModels
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;

    using Models;
    using Common;
    using Common.Dtos.ProductStandard;
   
    /// <summary>
    /// Repositorio para los productos standards
    /// </summary>
    public interface IProductStandardRepositoryModel
    {
        /// <summary>
        /// Agrega un nuevo producto.
        /// </summary>
        /// <param name="productStandard"></param>
        /// <returns></returns>
        public Task AddProductStandardAsync(ProductStandard productStandard);

        /// <summary>
        /// Actualiza un nuevo producto.
        /// </summary>
        /// <param name="productStandard"></param>
        /// <returns></returns>
        public Task<ProductStandard> UpdateProductStandardAsync(ProductStandard productStandard);

        /// <summary>
        /// Obtiene un producto sin sus relaciones con otras entidades.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public Task<ProductStandard> GetProductStandardForIdNotIncludeAsync(int Id);
        
        /// <summary>
        /// Obtiene un producto con todas las relaciones de entidades.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public Task<ProductStandard> GetProductStandardForIdWithAllIncludeAsync(int Id);

        /// <summary>
        /// Obtiene un producto con las relaciones de categorias.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public Task<ProductStandard> GetProductStandardForIdWithCategoryAsync(int Id);

        /// <summary>
        /// Obtiene todos los productos con las relaciones de categorias.
        /// </summary>
        /// <returns></returns>
        public Task<List<ProductStandard>> GetAllProductStandardWithCategoryAsync();

        /// <summary>
        /// Obtiene todos los productos con todas las relaciones de entidades.
        /// </summary>
        /// <returns></returns>
        public Task<List<ProductStandard>> GetAllProductStandardWithAllIncludeAsync();

        /// <summary>
        /// Obtiene el Id del último producto disponible.
        /// </summary>
        /// <returns></returns>
        public Task<int> GetIdOfLastProductStandardAsync();

        /// <summary>
        /// Agrega imagenes para un producto.
        /// </summary>
        /// <param name="formFile"></param>
        /// <param name="productStandard"></param>
        /// <returns></returns>
        public Task<bool> AddImageForProductStandardAsync(IFormFile formFile, ProductStandard productStandard);

        /// <summary>
        /// Agrega imagenes para un producto.
        /// </summary>
        /// <param name="Image"></param>
        /// <param name="productStandard"></param>
        /// <returns></returns>
        public Task<bool> AddImageForProductStandardAsync(byte[] Image, ProductStandard productStandard);

        /// <summary>
        /// Borra la imagen de un producto.
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public Task DeleteImageProductStandardAsync(ImageProductStandard image);

        /// <summary>
        /// Accede a una imagen determinada de un producto.
        /// </summary>
        /// <param name="productStandard"></param>
        /// <param name="ImageId"></param>
        /// <returns></returns>
        public Task<ImageProductStandard> GetImageProductStandardAsync(ProductStandard productStandard, int ImageId);

        /// <summary>
        /// Accede a todas las imagenes de un producto.
        /// </summary>
        /// <param name="productStandard"></param>
        /// <returns></returns>
        public Task<List<ImageProductStandard>> GetAllImageProductStandardAsync(ProductStandard productStandard);

        /// <summary>
        /// Obtiene el Id de la ultima imagen que se agrego de un producto.
        /// </summary>
        /// <returns></returns>
        public Task<int> GetIdLastImageProductStandardAsync(ProductStandard productStandard);

    }
}
