namespace Isabella.Web.Models.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    /// <summary>
    /// Ordenes del tipo fast(Para pedidos informales, o sea usuarios que no desean ser clientes oficiales)
    /// </summary>
    public class Order : IEntity
    {
        /// <summary>
        /// Key
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// User.
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// Detalle de ordenes.
        /// </summary>
        public ICollection<OrderDetail> OrderDetails { get; set; }

        /// <summary>
        /// Gps
        /// </summary>
        public Gps Gps { get; set; }

        /// <summary>
        /// Gps
        /// </summary>
        [Phone(ErrorMessage = "El formato del número de telefono no es valido.")]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Dirección donde entregar la orden
        /// </summary>
        [Required(ErrorMessage = "Debe definir la dirección de entrega del pedido.")]
        public string Address { get; set; }

        /// <summary>
        /// Dirección donde entregar la orden
        /// </summary>
        [Required(ErrorMessage = "Debe definir un nombre o un alias de quién solicitó el pedido.")]
        public string AskForWho { get; set; }

        /// <summary>
        /// Fecha en que se realizó el pedido.
        /// </summary>
        [Required]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd hh:mm tt}", ApplyFormatInEditMode = false)]
        public DateTime OrderDate { get; set; }

        /// <summary>
        /// Tiempo en el que se debe entregar el pedido.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd hh:mm tt}", ApplyFormatInEditMode = false)]
        public DateTime? DeliveryDate { get; set; }

        /// <summary>
        /// Cantidad de productos sin repetir(o sea productos diferentes) que tiene nuestro carrito.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public int Lines
        {
            get
            {
                return OrderDetails.Count;
            }
        }

        /// <summary>
        /// Cantidad Total de Productos.
        /// </summary>
        public int QuantityTotalProductCombined
        {
            get
            {
                if(OrderDetails != null)
                {
                   if (!OrderDetails.Any())
                   return 0;
                   else
                   return this.OrderDetails.Sum(c => c.ProductCombined.Quantity);
                }
                else
                return 0;
            }
        }

        /// <summary>
        /// Cantidad Total de Agregados.
        /// </summary>
        public int QuantityTotalAggregate
        {
            get
            {
                if(OrderDetails != null)
                {
                    if (!OrderDetails.Any())
                    return 0;
                    else
                    {
                        var cant_aggregates = OrderDetails.Select(c => c.ProductCombined.CantAggregates);
                        if(cant_aggregates != null)
                        {
                            if (cant_aggregates.Any())
                            return this.OrderDetails.Sum(c => c.ProductCombined.CantAggregates.Sum(x => x.Quantity));
                            else
                            return 0;
                        }
                        return 0;
                    }
                }
                else
                return 0;
            }
        }

        /// <summary>
        /// Precio total en agregados
        /// </summary>
        public decimal PriceTotalOfAggregates
        {
            get
            {
                if (OrderDetails != null)
                {
                    if (!OrderDetails.Any())
                    return 0;
                    else
                    {
                        var cant_aggregates = OrderDetails.Select(c => c.ProductCombined.CantAggregates);
                        if (cant_aggregates != null)
                        {
                            if (cant_aggregates.Any())
                            return this.OrderDetails.Sum(c => c.ProductCombined.CantAggregates.Sum(x => x.PriceTotal));
                            else
                            return 0;
                        }
                        else
                        return 0;
                    }
                }
                else
                return 0;
            }
        }

        /// <summary>
        /// Precio Total del posible pedido.
        /// </summary>
        public decimal PriceTotal
        {
            get
            {
                if(OrderDetails != null)
                {
                    if (!OrderDetails.Any())
                    return 0;
                    else
                    return OrderDetails.Sum(c => c.PriceTotal);
                }
                else
                return 0;
            }
        }
    }
}
