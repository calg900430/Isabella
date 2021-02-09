namespace Isabella.API.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using Models;

    /// <summary>
    /// DataContext
    /// </summary>
    public class DataContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        /// <summary>
        /// Constructor del Context
        /// </summary>
        /// <param name="dbContextOptions"></param>
        public DataContext(DbContextOptions<DataContext> dbContextOptions) : base(dbContextOptions)
        {

        }

        /// <summary>
        /// Códigos de Verificación.
        /// </summary>
        public DbSet<CodeIdentification> CodeIdentifications { get; set; }

        /// <summary>
        /// Confirmación de registro de usuario.
        /// </summary>
        public DbSet<ConfirmationRegisterForEmail> ConfirmationRegisterForEmail { get; set; }

        /// <summary>
        /// Recuperación de contraseña.
        /// </summary>
        public DbSet<RecoverPassword> RecoverPasswords { get; set; }

        /// <summary>
        /// Coordenadas Gps.
        /// </summary>
        public DbSet<Gps> Gps { get; set; }

        /// <summary>
        /// Pedidos de los usuarios.
        /// </summary>
        public DbSet<Order> Orders { get; set; }

        /// <summary>
        /// Detalles de los pedidos.
        /// </summary>
        public DbSet<OrderDetail> OrderDetails { get; set; }

        /// <summary>
        /// Calificación de un producto special.
        /// </summary>
        public DbSet<CalificationProductSpecial> CalificationProductSpecials { get; set; }

        /// <summary>
        /// Calificación de Productos Standards.
        /// </summary>
        public DbSet<CalificationProductStandard> CalificationProductStandards { get; set; }

        /// <summary>
        /// Calificacion
        /// </summary>
        public DbSet<CalificationRestaurant> CalificationRestaurants { get; set; }

        /// <summary>
        /// Carrito de compra de productos standards
        /// </summary>
        public DbSet<CarShopProductStandard> CarShopsProductsStandards { get; set; }

        /// <summary>
        /// Carrito de compra de productos especiales
        /// </summary>
        public DbSet<CarShopProductSpecial> CarShopsProductsSpecials { get; set; }

        /// <summary>
        /// Productos de agregos.
        /// </summary>
        public DbSet<ProductAggregate> ProductAggregates { get; set; }

        /// <summary>
        /// Productos Specials.
        /// </summary>
        public DbSet<ProductSpecial> ProductsSpecials { get; set; }

        /// <summary>
        /// Productos Standards.
        /// </summary>
        public DbSet<ProductStandard> ProductsStandards { get; set; }

        /// <summary>
        /// Imagenes de productos especiales.
        /// </summary>
        public DbSet<ImageProductSpecial> ImageProductSpecials { get; set; }

        /// <summary>
        /// Imagenes de productos standards.
        /// </summary>
        public DbSet<ImageProductStandard> ImageProductStandards { get; set; }

        /// <summary>
        /// Imagenes de productos de agrego.
        /// </summary>
        public DbSet<ImageProductAggregate> ImageProductAggregates { get; set; }

        /// <summary>
        /// Pedidos de productos de agrego para un producto special.
        /// </summary>
        public DbSet<CarShopProductAggregate> CarShopProductAggregates { get; set; }
        
        /// <summary>
        /// Categoria de los productos standards.
        /// </summary>
        public DbSet<Category> Categories { get; set; }

        /// <summary>
        /// SubCategorias.
        /// </summary>
        public DbSet<SubCategory> SubCategories { get; set; }

        /// <summary>
        /// SubCategoria de Productos Standards.
        /// </summary>
        public DbSet<SubCategory_ProductStandard> SubCategory_ProductStandards { get; set; }

        /// <summary>
        /// SubCategoria de Productos Speciales.
        /// </summary>
        public DbSet<SubCategory_ProductSpecial> SubCategory_ProductSpecials { get; set; }

        /// <summary>
        /// Crea el modelo para la base de datos
        /// </summary>
        /// <param name="builder"></param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
