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
    using Common.Dtos.ProductSpecial;
    using Common.Extras;
    using Data;
    using Extras;
    using Models;
    using RepositorysModels;

    /// <summary>
    /// 
    /// </summary>
    public class ProductSpecialServiceModel : IProductSpecialRepositoryModel
    {
        private readonly DataContext _dataContext;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataContext"></param>
        public ProductSpecialServiceModel(DataContext dataContext)
        {
            this._dataContext = dataContext;
        }

        /// <summary>
        /// Agrega un nuevo producto.
        /// </summary>
        /// <param name="productSpecial"></param>
        /// <returns></returns>
        public async Task AddProductSpecialAsync(ProductSpecial productSpecial)
        {
            await this._dataContext.ProductsSpecials.AddAsync(productSpecial).ConfigureAwait(false);
            await this._dataContext.SaveChangesAsync();
        }

        /// <summary>
        /// Actualiza un producto.
        /// </summary>
        /// <param name="productSpecial"></param>
        /// <returns></returns>
        public async Task<ProductSpecial> UpdateProductSpecialAsync(ProductSpecial productSpecial)
        {
            this._dataContext.ProductsSpecials.Update(productSpecial);
            await this._dataContext.SaveChangesAsync();
            return productSpecial;
        }

        /// <summary>
        /// Obtiene un producto sin sus relaciones con otras entidades.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<ProductSpecial> GetProductSpecialForIdNotIncludeAsync(int Id)
        => await this._dataContext.ProductsSpecials
        .FirstOrDefaultAsync(c => c.Id == Id)
        .ConfigureAwait(false);

        /// <summary>
        /// Obtiene un producto con todas las relaciones de entidades.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<ProductSpecial> GetProductSpecialForIdWithAllIncludeAsync(int Id)
         => await this._dataContext.ProductsSpecials
        .Include(c => c.CategoryProductSpecial)
        .Include(c => c.ImageProductSpecials)
        .FirstOrDefaultAsync(c => c.Id == Id)
        .ConfigureAwait(false);

        /// <summary>
        /// Obtiene un producto con las relaciones de categorias.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<ProductSpecial> GetProductSpecialForIdWithCategoryAsync(int Id)
         => await this._dataContext.ProductsSpecials
        .Include(c => c.CategoryProductSpecial)
        .FirstOrDefaultAsync(c => c.Id == Id)
        .ConfigureAwait(false);

        /// <summary>
        /// Agrega imagenes para un producto.
        /// </summary>
        /// <param name="formFile"></param>
        /// <param name="productSpecial"></param>
        /// <returns></returns>
        public async Task<bool> AddImageForProductSpecialAsync(IFormFile formFile, ProductSpecial productSpecial)
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
            var image_product = new ImageProductSpecial
            {
                Image = arraybyte_image,
                ProductSpecial = productSpecial
            };
            //Guarda el registro
            await this._dataContext.ImageProductSpecials.AddAsync(image_product).ConfigureAwait(false);
            await this._dataContext.SaveChangesAsync().ConfigureAwait(false);
            //Elimina la imagen
            System.IO.File.Delete(path);
            return true;
        }

        /// <summary>
        /// Agrega imagenes para un producto.
        /// </summary>
        /// <param name="Image"></param>
        /// <param name="productSpecial"></param>
        /// <returns></returns>
        public async Task<bool> AddImageForProductSpecialAsync(byte[] Image, ProductSpecial productSpecial)
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
            var image_product = new ImageProductSpecial
            {
                Image = arraybyte_image,
                ProductSpecial = productSpecial
            };
            //Guarda el registro
            await this._dataContext.ImageProductSpecials.AddAsync(image_product).ConfigureAwait(false);
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
        public async Task DeleteImageProductSpecialAsync(ImageProductSpecial image)
        {
            //Elimina la imagen de la base de datos.
            this._dataContext.ImageProductSpecials.Remove(image);
            //Guarda los cambios en la base de datos.
            await this._dataContext.SaveChangesAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Accede a todas las imagenes de un producto.
        /// </summary>
        /// <param name="productSpecial"></param>
        /// <returns></returns>
        public async Task<List<ImageProductSpecial>> GetAllImageProductSpecialAsync(ProductSpecial productSpecial)
        => await this._dataContext.ImageProductSpecials
        .Include(c => c.ProductSpecial)
        .Where(c => c.ProductSpecial == productSpecial)
        .ToListAsync()
        .ConfigureAwait(false);

        /// <summary>
        /// Obtiene todos los productos con todas las relaciones de entidades.
        /// </summary>
        /// <returns></returns>
        public async Task<List<ProductSpecial>> GetAllProductSpecialWithAllIncludeAsync()
        => await this._dataContext.ProductsSpecials
        .Include(c => c.CategoryProductSpecial)
        .Include(c => c.ImageProductSpecials)
        .ToListAsync()
        .ConfigureAwait(false);

        /// <summary>
        /// Obtiene todos los productos con las relaciones de categorias.
        /// </summary>
        /// <returns></returns>
        public async Task<List<ProductSpecial>> GetAllProductSpecialWithCategoryAsync()
        => await this._dataContext.ProductsSpecials
        .Include(c => c.CategoryProductSpecial)
        .ToListAsync()
        .ConfigureAwait(false);

        /// <summary>
        /// Obtiene el Id de la ultima imagen que se agrego de un producto.
        /// </summary>
        /// <param name="productSpecial"></param>
        /// <returns></returns>
        public async Task<int> GetIdLastImageProductSpecialAsync(ProductSpecial productSpecial)
        {
            var all_imagen = await this._dataContext.ImageProductSpecials
            .Where(c => c.ProductSpecial == productSpecial)
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
        public async Task<int> GetIdOfLastProductSpecialAsync()
        {
            var list_products = await this._dataContext.ProductsSpecials.ToListAsync().ConfigureAwait(false);
            if (list_products == null)
            return -1;
            return list_products.LastOrDefault().Id;
        }

        /// <summary>
        /// Accede a una imagen determinada de un producto.
        /// </summary>
        /// <param name="productSpecial"></param>
        /// <param name="ImageId"></param>
        /// <returns></returns>
        public async Task<ImageProductSpecial> GetImageProductSpecialAsync(ProductSpecial productSpecial, int ImageId)
        {
            var image = await this._dataContext.ImageProductSpecials
           .Include(c => c.ProductSpecial)
           .Where(c => c.Id == ImageId && c.ProductSpecial == productSpecial)
           .FirstOrDefaultAsync()
           .ConfigureAwait(false);
            if (image == null)
            return null;
            return image;
        }
    }
}
