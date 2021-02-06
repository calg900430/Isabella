namespace Isabella.API.RepositorysModels
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;

    using Models;
    using Common;
    using Common.Dtos.ProductAggregate;

    /// <summary>
    /// 
    /// </summary>
    public interface IProductAggregateRepositoryModel
    {
        /// <summary>
        /// Agrega un nuevo producto.
        /// </summary>
        /// <param name="productAggregate"></param>
        /// <returns></returns>
        public Task AddProductAggregateAsync(ProductAggregate productAggregate);

        /// <summary>
        /// Actualiza un producto.
        /// </summary>
        /// <param name="productAggregate"></param>
        /// <returns></returns>
        public Task<ProductAggregate> UpdateProductAggregateAsync(ProductAggregate productAggregate);

        /// <summary>
        /// Obtiene un producto sin sus relaciones con otras entidades.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public Task<ProductAggregate> GetProductAggregateForIdNotIncludeAsync(int Id);

        /// <summary>
        /// Obtiene un producto con todas las relaciones de entidades.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public Task<ProductAggregate> GetProductAggregateForIdWithAllIncludeAsync(int Id);

        /// <summary>
        /// Obtiene un producto con las relaciones de categorias.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public Task<ProductAggregate> GetProductAggregateForIdWithCategoryAsync(int Id);

        /// <summary>
        /// Obtiene todos los productos con las relaciones de categorias.
        /// </summary>
        /// <returns></returns>
        public Task<List<ProductAggregate>> GetAllProductAggregateWithCategoryAsync();

        /// <summary>
        /// Obtiene todos los productos con todas las relaciones de entidades.
        /// </summary>
        /// <returns></returns>
        public Task<List<ProductAggregate>> GetAllProductAggregateWithAllIncludeAsync();

        /// <summary>
        /// Obtiene el Id del último producto disponible.
        /// </summary>
        /// <returns></returns>
        public Task<int> GetIdOfLastProductAggregateAsync();

        /// <summary>
        /// Agrega imagenes para un producto.
        /// </summary>
        /// <param name="formFile"></param>
        /// <param name="productAggregate"></param>
        /// <returns></returns>
        public Task<bool> AddImageForProductAggregateAsync(IFormFile formFile, ProductAggregate productAggregate);

        /// <summary>
        /// Agrega imagenes para un producto.
        /// </summary>
        /// <param name="Image"></param>
        /// <param name="productAggregate"></param>
        /// <returns></returns>
        public Task<bool> AddImageForProductAggregateAsync(byte[] Image, ProductAggregate productAggregate);

        /// <summary>
        /// Borra la imagen de un producto.
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public Task DeleteImageProductAggregateAsync(ImageProductAggregate image);

        /// <summary>
        /// Accede a una imagen determinada de un producto.
        /// </summary>
        /// <param name="productAggregate"></param>
        /// <param name="ImageId"></param>
        /// <returns></returns>
        public Task<ImageProductAggregate> GetImageProductAggregateAsync(ProductAggregate productAggregate, int ImageId);

        /// <summary>
        /// Accede a todas las imagenes de un producto.
        /// </summary>
        /// <param name="productAggregate"></param>
        /// <returns></returns>
        public Task<List<ImageProductAggregate>> GetAllImageProductAggregateAsync(ProductAggregate productAggregate);

        /// <summary>
        /// Obtiene el Id de la ultima imagen que se agrego de un producto.
        /// </summary>
        /// <param name="productAggregate"></param>
        /// <returns></returns>
        public Task<int> GetIdLastImageProductAggregateAsync(ProductAggregate productAggregate);
    }
}
