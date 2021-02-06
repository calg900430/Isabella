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
    using Common.Dtos.CategoryProductStandard;
    using Common.Dtos.ProductStandard;
    using Common.Extras;
    using Data;
    using Extras;
    using Models;
    using RepositorysModels;

    /// <summary>
    /// Servicio para la entidad que representa los productos standard.
    /// </summary>
    public class ProductStandardServiceModel : IProductStandardRepositoryModel
    {
        private readonly DataContext _dataContext;
       
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="dataContext"></param>
        public ProductStandardServiceModel(DataContext dataContext)
        {
            this._dataContext = dataContext;
        }

        /// <summary>
        /// Agrega un nuevo producto.
        /// </summary>
        /// <param name="productStandard"></param>
        /// <returns></returns>
        public async Task AddProductStandardAsync(ProductStandard productStandard)
        {
            await this._dataContext.ProductsStandards.AddAsync(productStandard).ConfigureAwait(false);
            await this._dataContext.SaveChangesAsync();
        }

        /// <summary>
        /// Actualiza un nuevo producto.
        /// </summary>
        /// <param name="productStandard"></param>
        /// <returns></returns>
        public async Task<ProductStandard> UpdateProductStandardAsync(ProductStandard productStandard)
        {
            this._dataContext.ProductsStandards.Update(productStandard);
            await this._dataContext.SaveChangesAsync();
            return productStandard;
        }

        /// <summary>
        /// Obtiene un producto con las relaciones de categorias.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<ProductStandard> GetProductStandardForIdWithCategoryAsync(int Id)
         => await this._dataContext.ProductsStandards
        .Include(c => c.CategoryProductStandard)
        .FirstOrDefaultAsync(c => c.Id == Id)
        .ConfigureAwait(false);

        /// <summary>
        /// Obtiene todos los productos con las relaciones de categorias.
        /// </summary>
        /// <returns></returns>
        public async Task<List<ProductStandard>> GetAllProductStandardWithCategoryAsync()
        => await this._dataContext.ProductsStandards
        .Include(c => c.CategoryProductStandard)
        .ToListAsync()
        .ConfigureAwait(false);

        /// <summary>
        /// Obtiene un producto sin sus relaciones con otras entidades.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<ProductStandard> GetProductStandardForIdNotIncludeAsync(int Id)
        => await this._dataContext.ProductsStandards
        .FirstOrDefaultAsync(c => c.Id == Id)
        .ConfigureAwait(false);

        /// <summary>
        /// Obtiene todos los productos disponibles con su relación de categoría y imagenes.
        /// </summary>
        /// <returns></returns>
        public async Task<List<ProductStandard>> GetAllProductStandardWithAllIncludeAsync()
        => await this._dataContext.ProductsStandards
        .Include(c => c.CategoryProductStandard)
        .Include(c => c.ImageProductStandards)
        .ToListAsync()
        .ConfigureAwait(false);

        /// <summary>
        /// Obtiene un producto con su relación de categoría dado su Id y imagenes.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<ProductStandard> GetProductStandardForIdWithAllIncludeAsync(int Id)
        => await this._dataContext.ProductsStandards
        .Include(c => c.CategoryProductStandard)
        .Include(c => c.ImageProductStandards)
        .FirstOrDefaultAsync(c => c.Id == Id)
        .ConfigureAwait(false);

        /// <summary>
        /// Obtiene el Id del último producto disponible.
        /// </summary>
        /// <returns></returns>
        public async Task<int> GetIdOfLastProductStandardAsync()
        { 
           var list_products = await this._dataContext.ProductsStandards.ToListAsync().ConfigureAwait(false);
           if (list_products == null)
           return -1;
           return list_products.LastOrDefault().Id;
        }

        /// <summary>
        /// Agrega imagenes para un producto.
        /// </summary>
        /// <param name="formFile"></param>
        /// <param name="productStandard"></param>
        /// <returns></returns>
        public async Task<bool> AddImageForProductStandardAsync(IFormFile formFile, ProductStandard productStandard)
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
            var image_product = new ImageProductStandard
            {
                Image = arraybyte_image,
                ProductStandard = productStandard
            };
            //Guarda el registro
            await this._dataContext.ImageProductStandards.AddAsync(image_product).ConfigureAwait(false);
            await this._dataContext.SaveChangesAsync().ConfigureAwait(false);
            //Elimina la imagen
            System.IO.File.Delete(path);
            return true;
        }

        /// <summary>
        /// Agrega imagenes para un producto.
        /// </summary>
        /// <param name="Image"></param>
        /// <param name="productStandard"></param>
        /// <returns></returns>
        public async Task<bool> AddImageForProductStandardAsync(byte[] Image, ProductStandard productStandard)
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
            var image_product = new ImageProductStandard
            {
                Image = arraybyte_image,
                ProductStandard = productStandard
            };
            //Guarda el registro
            await this._dataContext.ImageProductStandards.AddAsync(image_product).ConfigureAwait(false);
            await this._dataContext.SaveChangesAsync().ConfigureAwait(false);
            //Elimina la imagen
            System.IO.File.Delete(path);
            return true;
        }

        /// <summary>
        /// Borra la imagen de un producto.
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public async Task DeleteImageProductStandardAsync(ImageProductStandard image)
        {
            //Elimina la imagen de la base de datos.
            this._dataContext.ImageProductStandards.Remove(image);
            //Guarda los cambios en la base de datos.
            await this._dataContext.SaveChangesAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Verifica si la imagen pertenece al producto.
        /// </summary>
        /// <param name="productStandard"></param>
        /// <param name="ImageId"></param>
        /// <returns></returns>
        public async Task<ImageProductStandard> GetImageProductStandardAsync(ProductStandard productStandard, int ImageId)
        {
            var image = await this._dataContext.ImageProductStandards
            .Include(c => c.ProductStandard)
            .Where(c => c.Id == ImageId && c.ProductStandard == productStandard)
            .FirstOrDefaultAsync()
            .ConfigureAwait(false);
            if (image == null)
            return null;
            return image;
        }

        /// <summary>
        /// Accede a todas las imagenes de un producto.
        /// </summary>
        /// <param name="productStandard"></param>
        /// <returns></returns>
        public async Task<List<ImageProductStandard>> GetAllImageProductStandardAsync(ProductStandard productStandard)
        => await this._dataContext.ImageProductStandards
        .Include(c => c.ProductStandard)
        .Where(c => c.ProductStandard == productStandard)
        .ToListAsync()
        .ConfigureAwait(false);

        /// <summary>
        /// Obtiene el Id de la ultima imagen que se agrego de un producto.
        /// </summary>
        /// <returns></returns>
        public async Task<int> GetIdLastImageProductStandardAsync(ProductStandard productStandard)
        {
           var all_imagen = await this._dataContext.ImageProductStandards
           .Where(c => c.ProductStandard == productStandard)
           .ToListAsync()
           .ConfigureAwait(false);
           if(all_imagen == null)
           return -1;
           return all_imagen.Last().Id;
        }

    }
}
