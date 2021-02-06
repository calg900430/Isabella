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
        public DbSet<ConfirmationEmail> ConfirmationsEmails { get; set; }
        public DbSet<RecoverPassword> RecoverPasswords { get; set; }
        public DbSet<Gps> Gps { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<CalificationProductSpecial> CalificationProductSpecials { get; set; }
        public DbSet<CalificationProductStandard> CalificationProductStandards { get; set; }
        public DbSet<CalificationRestaurant> CalificationRestaurants { get; set; }
        public DbSet<CarShop> CarShops { get; set; }
        public DbSet<ImageProductStandard> ProductImages { get; set; }
        public DbSet<ProductTypeCheese> ProductTypeCheese { get; set; }
        public DbSet<ProductTypeAggregate> ProductTypeAggregates { get; set; }
        public DbSet<ProductSpecial> ProductsSpecials { get; set; }
        public DbSet<ProductStandard> ProductsStandards { get; set; }
        public DbSet<ImageProductSpecial> ImageProductSpecials { get; set; }
        public DbSet<ImageProductStandard> ImageProductStandards { get; set; }
        public DbSet<ImageProductTypeAggregate> ImageProductTypeAggregates { get; set; }
        public DbSet<ImageProductTypeCheese> ImageProductTypeCheeses { get; set; }
        public DbSet<RequestedForUser_ProductTypeCheese> RequestedForUser_ProductTypeCheeses { get; set; }
        public DbSet<RequestedForUser_ProductTypeAggregate> RequestedForUser_ProductTypeAggregates { get; set; }
        public DbSet<RequestedForUser_ProductStandard> RequestedForUser_ProductStandards { get; set; }
        public DbSet<RequestedForUser_ProductSpecial> RequestedForUser_ProductSpecials { get; set; }
        public DbSet<CategoryProductSpecial> CategoryProductSpecials { get; set; }
        public DbSet<CategoryProductStandard> CategoryProductStandards { get; set; }
        public DbSet<CategoryProductTypeAggregate> CategoryProductTypeAggregates { get; set; }
        public DbSet<CategoryProductTypeCheese> CategoryProductTypeCheeses { get; set; }

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
