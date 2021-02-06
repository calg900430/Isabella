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
        /// 
        /// </summary>
        public DbSet<CalificationProductStandard> CalificationProductStandards { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DbSet<CalificationRestaurant> CalificationRestaurants { get; set; }

        /// <summary>
        /// Carrito de compra
        /// </summary>
        public DbSet<CarShop> CarShops { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DbSet<ImageProductStandard> ProductImages { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DbSet<ProductAggregate> ProductAggregates { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DbSet<ProductSpecial> ProductsSpecials { get; set; }

        /// <summary>
        /// Productos Standards.
        /// </summary>
        public DbSet<ProductStandard> ProductsStandards { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DbSet<ImageProductSpecial> ImageProductSpecials { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DbSet<ImageProductStandard> ImageProductStandards { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DbSet<ImageProductAggregate> ImageProductAggregates { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DbSet<RequestedProductAggregate> RequestedProductAggregates { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public DbSet<RequestedProductStandard> RequestedProductStandards { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public DbSet<RequestedProductSpecial> RequestedProductSpecials { get; set; }

        /// <summary>
        /// Categoria de los productos especiales.
        /// </summary>
        public DbSet<CategoryProductSpecial> CategoryProductSpecials { get; set; }

        /// <summary>
        /// Categoria de los productos standards.
        /// </summary>
        public DbSet<CategoryProductStandard> CategoryProductStandards { get; set; }

        /// <summary>
        /// Categoria de los productos de agregados.
        /// </summary>
        public DbSet<CategoryProductAggregate> CategoryProductAggregates { get; set; }

     
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
