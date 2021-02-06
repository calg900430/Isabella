namespace Isabella.Common.Dtos.Order
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Isabella.Common.Extras;

    public class GetAllOrdersOfUserDto
    {
        /// <summary>
        /// Usuario
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Coordenadas GPS
        /// </summary>
        public Gps Gps { get; set; }

        /// <summary>
        /// Dirección donde se debe entregar el pedido
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Obtiene todos los productos del pedido
        /// </summary>
        public IEnumerable<GetOrderDetailDto> Items { get; set; }

        /// <summary>
        /// Fecha en que realizó el usuario el pedido
        /// </summary>
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// Fecha en que se realiza la entrega del pedido.
        /// </summary>
        public DateTime? DateDelivery { get; set; }

        /// <summary>
        /// Cantidad de productos sin repetir(o se productos diferentes) que tiene nuestro pedido.
        /// </summary>
        public int Lines { get { return this.Items == null ? 0 : this.Items.Count(); } }

        /// <summary>
        /// Cantidad Total de todos los productos de la orden
        /// </summary>
        public double Quantity { get { return this.Items == null ? 0 : this.Items.Sum(i => i.Quantity); } }

        /// <summary>
        /// Precio Total de la Orden
        /// </summary>
        public decimal Value { get { return this.Items == null ? 0 : this.Items.Sum(i => i.Value); } }
    }
}
