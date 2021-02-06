namespace Isabella.Web.Repositorys.API
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System;
    using Common;
    using Common.Dtos.Product;
    using Models.Entities;
    using Common.Dtos.Product;

    /// <summary>
    /// Servicio para gestionar con la entidad productos.
    /// </summary>
    public interface IProductRepositoryAPI : IGenericRepository<Product>
    {
        /// <summary>
        /// Accede a todos los productos del restaurante.
        /// </summary>
        /// <returns></returns>
        Task<ServiceResponse<List<GetProductDto>>> GetAllProductAsync();

        /// <summary>
        /// Accede a un producto del restaurante por su Id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ServiceResponse<GetProductDto>> GetProductByIdAsync(int id);

        /// <summary>
        /// Agrega un nuevo producto.
        /// </summary>
        /// <param name="addProduct"></param>
        /// <returns></returns>
        Task<ServiceResponse<bool>> AddProductAsync(AddProductDto addProduct);

        /// <summary>
        /// Agrega una calificación a un producto del restaurante.
        /// </summary>
        /// <param name="calificationProduct"></param>
        /// <returns></returns>
        Task<ServiceResponse<bool>> AddCalificationForProductAsync(AddCalificationProductDto calificationProduct);
    }
}
