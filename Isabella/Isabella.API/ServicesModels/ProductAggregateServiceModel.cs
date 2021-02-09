namespace Isabella.API.ServicesModels
{
    using System;
    using System.IO;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.EntityFrameworkCore;

    using Common;
    using Common.Dtos.ProductAggregate;
    using Common.Extras;
    using Data;
    using Extras;
    using Models;
    using RepositorysModels;

    /// <summary>
    /// 
    /// </summary>
    public class ProductAggregateServiceModel : IProductAggregateRepositoryModel
    {
        private readonly DataContext _dataContext;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="dataContext"></param>
        public ProductAggregateServiceModel(DataContext dataContext)
        {
            this._dataContext = dataContext;
        }

        /// <summary>
        /// Agrega imagenes para un producto.
        /// </summary>
        /// <param name="formFile"></param>
        /// <param name="productAggregate"></param>
        /// <returns></returns>
        public async Task<bool> AddImageForProductAggregateAsync(IFormFile formFile, ProductAggregate productAggregate)
        {
            //Nombre de la imagen
            var file = $"{Guid.NewGuid()}.jpg";
            //Ruta temporal donde la guardaremos antes de enviarla a la base de datos.
            var path = Path.Combine(Directory.GetCurrentDirectory(), file);
            //Crea el archivo de la imagen que se encuentra en memoria RAM y lo guarda en la ruta seleccionada.
            using (var stream = new FileStream(path, FileMode.Create))
            {
                await formFile.CopyToAsync(stream).ConfigureAwait(false);
            };
            //Verifica si se guardo la imagen.
            var image_exiting = System.IO.File.Exists(path);
            if (!image_exiting)
                return false;
            //Obtiene el archivo de imagen
            var arraybyte_image = System.IO.File.ReadAllBytes(path);
            if (arraybyte_image.Length <= 0)
                return false;
            //Crea el registro que contiene la nueva imagen del producto.
            var image_product = new ImageProductAggregate
            {
                Image = arraybyte_image,
                ProductAggregate = productAggregate
            };
            //Guarda el registro
            await this._dataContext.ImageProductAggregates.AddAsync(image_product).ConfigureAwait(false);
            await this._dataContext.SaveChangesAsync().ConfigureAwait(false);
            //Elimina la imagen
            System.IO.File.Delete(path);
            return true;
        }

        /// <summary>
        /// Agrega imagenes para un producto.
        /// </summary>
        /// <param name="Image"></param>
        /// <param name="productAggregate"></param>
        /// <returns></returns>
        public async Task<bool> AddImageForProductAggregateAsync(byte[] Image, ProductAggregate productAggregate)
        {
            //Nombre de la imagen
            var file = $"{Guid.NewGuid()}.jpg";
            //Ruta temporal donde la guardaremos antes de enviarla a la base de datos.
            var path = Path.Combine(Directory.GetCurrentDirectory(), file);
            //Crea el archivo de la imagen que se encuentra en memoria RAM y lo guarda en la ruta seleccionada.
            File.WriteAllBytes(path, Image);
            //Verifica si se guardo la imagen.
            var image_exiting = System.IO.File.Exists(path);
            if (!image_exiting)
                return false;
            //Obtiene el archivo de imagen
            var arraybyte_image = System.IO.File.ReadAllBytes(path);
            if (arraybyte_image.Length <= 0)
                return false;
            //Crea el registro que contiene la nueva imagen del producto.
            var image_product = new ImageProductAggregate
            {
                Image = arraybyte_image,
                ProductAggregate = productAggregate
            };
            //Guarda el registro
            await this._dataContext.ImageProductAggregates.AddAsync(image_product).ConfigureAwait(false);
            await this._dataContext.SaveChangesAsync().ConfigureAwait(false);
            //Elimina la imagen
            System.IO.File.Delete(path);
            return true;
        }

        /// <summary>
        /// Agrega un nuevo producto.
        /// </summary>
        /// <param name="productAggregate"></param>
        /// <returns></returns>
        public async Task AddProductAggregateAsync(ProductAggregate productAggregate)
        {
            await this._dataContext.ProductAggregates.AddAsync(productAggregate).ConfigureAwait(false);
            await this._dataContext.SaveChangesAsync();
        }

        /// <summary>
        /// Borra la imagen de un producto.
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public async Task DeleteImageProductAggregateAsync(ImageProductAggregate image)
        {
            //Elimina la imagen de la base de datos.
            this._dataContext.ImageProductAggregates.Remove(image);
            //Guarda los cambios en la base de datos.
            await this._dataContext.SaveChangesAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Accede a todas las imagenes de un producto.
        /// </summary>
        /// <param name="productAggregate"></param>
        /// <returns></returns>
        public async Task<List<ImageProductAggregate>> GetAllImageProductAggregateAsync(ProductAggregate productAggregate)
         => await this._dataContext.ImageProductAggregates
        .Include(c => c.ProductAggregate)
        .Where(c => c.ProductAggregate == productAggregate)
        .ToListAsync()
        .ConfigureAwait(false);

        /// <summary>
        /// Obtiene todos los productos con todas las relaciones de entidades.
        /// </summary>
        /// <returns></returns>
        public async Task<List<ProductAggregate>> GetAllProductAggregateWithAllIncludeAsync()
        => await this._dataContext.ProductAggregates
        .Include(c => c.Category)
        .Include(c => c.ImageProductAggregates)
        .ToListAsync();

        /// <summary>
        /// Obtiene todos los productos con las relaciones de categorias.
        /// </summary>
        /// <returns></returns>
        public async Task<List<ProductAggregate>> GetAllProductAggregateWithCategoryAsync()
          => await this._dataContext.ProductAggregates
        .Include(c => c.Category)
        .ToListAsync()
        .ConfigureAwait(false);

        /// <summary>
        /// Obtiene el Id de la ultima imagen que se agrego de un producto.
        /// </summary>
        /// <param name="productAggregate"></param>
        /// <returns></returns>
        public async Task<int> GetIdLastImageProductAggregateAsync(ProductAggregate productAggregate)
        {
            var all_imagen = await this._dataContext.ImageProductAggregates
            .Where(c => c.ProductAggregate == productAggregate)
            .ToListAsync()
            .ConfigureAwait(false);
            if (all_imagen == null)
            return -1;
            return all_imagen.Last().Id;
        }

        /// <summary>
        /// Obtiene el Id del último producto disponible.
        /// </summary>
        /// <returns></returns>
        public async Task<int> GetIdOfLastProductAggregateAsync()
        {
            var list_products = await this._dataContext.ProductAggregates
            .ToListAsync()
            .ConfigureAwait(false);
            if (list_products == null)
            return -1;
            return list_products.LastOrDefault().Id;
        }

        /// <summary>
        /// Accede a una imagen determinada de un producto.
        /// </summary>
        /// <param name="productAggregate"></param>
        /// <param name="ImageId"></param>
        /// <returns></returns>
        public async Task<ImageProductAggregate> GetImageProductAggregateAsync(ProductAggregate productAggregate, int ImageId)
        {
            var image = await this._dataContext.ImageProductAggregates
           .Include(c => c.ProductAggregate)
           .Where(c => c.Id == ImageId && c.ProductAggregate == productAggregate)
           .FirstOrDefaultAsync()
           .ConfigureAwait(false);
            if (image == null)
            return null;
            return image;
        }

        /// <summary>
        /// Obtiene un producto sin sus relaciones con otras entidades.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<ProductAggregate> GetProductAggregateForIdNotIncludeAsync(int Id)
        => await this._dataContext.ProductAggregates
        .FirstOrDefaultAsync(c => c.Id == Id)
        .ConfigureAwait(false);

        /// <summary>
        /// Obtiene un producto con todas las relaciones de entidades.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<ProductAggregate> GetProductAggregateForIdWithAllIncludeAsync(int Id)
        => await this._dataContext.ProductAggregates
        .Include(c => c.Category)
        .Include(c => c.ImageProductAggregates)
        .FirstOrDefaultAsync(c => c.Id == Id)
        .ConfigureAwait(false);

        /// <summary>
        /// Obtiene un producto con las relaciones de categorias.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<ProductAggregate> GetProductAggregateForIdWithCategoryAsync(int Id)
         => await this._dataContext.ProductAggregates
        .Include(c => c.Category)
        .FirstOrDefaultAsync(c => c.Id == Id)
        .ConfigureAwait(false);

        /// <summary>
        /// Actualiza un producto.
        /// </summary>
        /// <param name="productAggregate"></param>
        /// <returns></returns>
        public async Task<ProductAggregate> UpdateProductAggregateAsync(ProductAggregate productAggregate)
        {
            this._dataContext.ProductAggregates.Update(productAggregate);
            await this._dataContext.SaveChangesAsync();
            return productAggregate;
        }
    }
}
