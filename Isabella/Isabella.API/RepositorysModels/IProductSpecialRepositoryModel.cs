namespace Isabella.API.RepositorysModels
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;

    using Models;
    using Common;
    using Common.Dtos.ProductSpecial;

    /// <summary>
    /// 
    /// </summary>
    public interface IProductSpecialRepositoryModel
    {
        /// <summary>
        /// Agrega un nuevo producto.
        /// </summary>
        /// <param name="productSpecial"></param>
        /// <returns></returns>
        public Task AddProductSpecialAsync(ProductSpecial productSpecial);

        /// <summary>
        /// Actualiza un producto.
        /// </summary>
        /// <param name="productSpecial"></param>
        /// <returns></returns>
        public Task<ProductSpecial> UpdateProductSpecialAsync(ProductSpecial productSpecial);

        /// <summary>
        /// Obtiene un producto sin sus relaciones con otras entidades.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public Task<ProductSpecial> GetProductSpecialForIdNotIncludeAsync(int Id);

        /// <summary>
        /// Obtiene un producto con todas las relaciones de entidades.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public Task<ProductSpecial> GetProductSpecialForIdWithAllIncludeAsync(int Id);

        /// <summary>
        /// Obtiene un producto con las relaciones de categorias.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public Task<ProductSpecial> GetProductSpecialForIdWithCategoryAsync(int Id);

        /// <summary>
        /// Obtiene todos los productos con las relaciones de categorias.
        /// </summary>
        /// <returns></returns>
        public Task<List<ProductSpecial>> GetAllProductSpecialWithCategoryAsync();

        /// <summary>
        /// Obtiene todos los productos con todas las relaciones de entidades.
        /// </summary>
        /// <returns></returns>
        public Task<List<ProductSpecial>> GetAllProductSpecialWithAllIncludeAsync();

        /// <summary>
        /// Obtiene el Id del último producto disponible.
        /// </summary>
        /// <returns></returns>
        public Task<int> GetIdOfLastProductSpecialAsync();

        /// <summary>
        /// Agrega imagenes para un producto.
        /// </summary>
        /// <param name="formFile"></param>
        /// <param name="productSpecial"></param>
        /// <returns></returns>
        public Task<bool> AddImageForProductSpecialAsync(IFormFile formFile, ProductSpecial productSpecial);

        /// <summary>
        /// Agrega imagenes para un producto.
        /// </summary>
        /// <param name="Image"></param>
        /// <param name="productSpecial"></param>
        /// <returns></returns>
        public Task<bool> AddImageForProductSpecialAsync(byte[] Image, ProductSpecial productSpecial);

        /// <summary>
        /// Borra la imagen de un producto.
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public Task DeleteImageProductSpecialAsync(ImageProductSpecial image);

        /// <summary>
        /// Accede a una imagen determinada de un producto.
        /// </summary>
        /// <param name="productSpecial"></param>
        /// <param name="ImageId"></param>
        /// <returns></returns>
        public Task<ImageProductSpecial> GetImageProductSpecialAsync(ProductSpecial productSpecial, int ImageId);

        /// <summary>
        /// Accede a todas las imagenes de un producto.
        /// </summary>
        /// <param name="productSpecial"></param>
        /// <returns></returns>
        public Task<List<ImageProductSpecial>> GetAllImageProductSpecialAsync(ProductSpecial productSpecial);

        /// <summary>
        /// Obtiene el Id de la ultima imagen que se agrego de un producto.
        /// </summary>
        /// <param name="productSpecial"></param>
        /// <returns></returns>
        public Task<int> GetIdLastImageProductSpecialAsync(ProductSpecial productSpecial);
    }
}
