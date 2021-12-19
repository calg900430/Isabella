namespace Isabella.Web.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;

    using Models.Entities;

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
        /// Coordenadas Gps.
        /// </summary>
        public DbSet<Gps> Gps { get; set; }

        /// <summary>
        /// Pedidos de los usuarios.
        /// </summary>
        public DbSet<Order> Orders { get; set; }

        /// <summary>
        /// Notificaciones para los usuarios admins.
        /// </summary>
        public DbSet<UserAdminsNotifications> UserAdminsNotifications { get; set; }

        /// <summary>
        /// Notificaciones Pendientes.
        /// </summary>
        public DbSet<NotificationPendients> NotificationPendients { get; set; }

        /// <summary>
        /// Detalles de los pedidos.
        /// </summary>
        public DbSet<OrderDetail> OrderDetails { get; set; }

        /// <summary>
        /// Calificación de Productos.
        /// </summary>
        public DbSet<CalificationProduct> CalificationProduct { get; set; }

        /// <summary>
        /// Calificacion
        /// </summary>
        public DbSet<CalificationRestaurant> CalificationRestaurants { get; set; }

        /// <summary>
        /// Carrito de compra de productos standards
        /// </summary>
        public DbSet<CartShop> CarShops { get; set; }

        /// <summary>
        /// Productos de agregos.
        /// </summary>
        public DbSet<Aggregate> Aggregates { get; set; }

        /// <summary>
        /// Productos.
        /// </summary>
        public DbSet<Product> Products { get; set; }

        /// <summary>
        /// Imagenes de productos.
        /// </summary>
        public DbSet<ImageProduct> ImagesProducts { get; set; }

        /// <summary>
        /// Imagenes Agregados.
        /// </summary>
        public DbSet<ImageAggregate> ImageAggregates { get; set; }
 
        /// <summary>
        /// Pedidos de productos de agrego para un producto.
        /// </summary>
        public DbSet<CantAggregate> CantAggregate { get; set; }
        
        /// <summary>
        /// Categoria de los productos.
        /// </summary>
        public DbSet<Category> Categories { get; set; }

        /// <summary>
        /// SubCategorias.
        /// </summary>
        public DbSet<SubCategory> SubCategories { get; set; }

        /// <summary>
        /// Producto Combinado.
        /// </summary>
        public DbSet<ProductCombined> ProductCombineds { get; set; }

        /// <summary>
        /// Restaurant
        /// </summary>
        public DbSet<Restaurant> Restaurants { get; set; }

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
