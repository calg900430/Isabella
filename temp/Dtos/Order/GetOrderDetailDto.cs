namespace Isabella.Common.Dtos.Order
{
    using Isabella.Common.Dtos.Product;
    using System;
    using System.Collections.Generic;

    public class GetOrderDetailDto
    {
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
