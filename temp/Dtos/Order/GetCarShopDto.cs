namespace Isabella.Common.Dtos.Order
{
    using System;
    using System.Collections.Generic;
    using Product;

    public class GetCarShopDto
    {
        /// <summary>
        /// UserId
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// CarsShopId
        /// </summary>
        public int CarsShop { get; set; }

        /// <summary>
        /// Productos que el usuario a agregado al carrito compras
        /// </summary>
        public List<GetProductDto> GetProducts { get; set; }

        /// <summary>
        /// Productos que el usuario a agregado al carrito compras
        /// </summary>
        public List<GetProduct_PizzasPastasDto> GetProduct_PizzasPastas { get; set; }
    }
}
