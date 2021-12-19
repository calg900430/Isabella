namespace Isabella.Common.Dtos.Order
{
    using Isabella.Common.Dtos.Users;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    public class GetAllOrderDto
    {
        /// <summary>
        /// User
        /// </summary>
        public GetUserDto GetUserDto { get; set; }

        /// <summary>
        /// Ordenes
        /// </summary>
        public List<GetOrderDto> GetAllOrders { get; set; }

        /// <summary>
        /// Fecha en que se realizó el pedido.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd hh:mm tt}", ApplyFormatInEditMode = false)]
        public DateTime? OrderDate { get; set; }

        /// <summary>
        /// Tiempo en el que se debe entregar el pedido.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd hh:mm tt}", ApplyFormatInEditMode = false)]
        public DateTime? DeliveryDate { get; set; }

        /// <summary>
        /// Cantidad Total de Productos.
        /// </summary>
        public int QuantityTotalProductCombinedOfOrder
        {
            get
            {
               if(GetAllOrders != null)
               {
                    if (!GetAllOrders.Any())
                    return 0;
                    else
                    return this.GetAllOrders.Sum(c => c.QuantityTotalProductCombined);
               }
               else
               return 0;
            }

        }

        /// <summary>
        /// Cantidad Total de Agregados.
        /// </summary>
        public int QuantityTotalAggregateOfOrder
        {
            get
            {
               if (GetAllOrders != null)
               {
                   if (!GetAllOrders.Any())
                   return 0;
                   else
                   return this.GetAllOrders.Sum(c => c.QuantityTotalAggregate);
               }
               else
               return 0;
            }
        }

        /// <summary>
        /// Precio total en agregados
        /// </summary>
        public decimal PriceTotalOfAggregatesOfOrder
        {
            get
            {
                if (GetAllOrders != null)
                {
                    if (!GetAllOrders.Any())
                    return 0;
                    else
                    return this.GetAllOrders.Sum(c => c.PriceTotalOfAggregates);
                }
                else
                return 0;
            }
        }

        /// <summary>
        /// Precio Total del posible pedido.
        /// </summary>
        public decimal PriceTotalOfOrder
        {
            get
            {
                if (GetAllOrders != null)
                {
                    if (!GetAllOrders.Any())
                    return 0;
                    else
                    return this.GetAllOrders.Sum(c => c.PriceTotal);
                }
                else
                return 0;
            }
        }

    }

    public class GetOrderDto
    {
        /// <summary>
        /// Key
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Productos
        /// </summary>
        public List<GetAllOrderDetail> GetAllOrderDetails { get; set; }

        /// <summary>
        /// Gps
        /// </summary>
        public GetGps GetGps { get; set; }

        /// <summary>
        /// Gps
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Dirección donde entregar la orden
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Dirección donde entregar la orden
        /// </summary>
        public string AskForWho { get; set; }

        /// <summary>
        /// Cantidad de productos sin repetir(o sea productos diferentes) que tiene nuestro carrito.
        /// </summary>
        public int Lines
        {
            get
            {
                return GetAllOrderDetails.Count;
            }
        }

        /// <summary>
        /// Cantidad Total de Productos.
        /// </summary>
        public int QuantityTotalProductCombined
        {
            get
            {
                if(GetAllOrderDetails != null)
                {
                    if (!GetAllOrderDetails.Any())
                    return 0;
                    else
                    return this.GetAllOrderDetails.Sum(c => c.Quantity);
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
                if (GetAllOrderDetails != null)
                {
                    if (!GetAllOrderDetails.Any())
                    return 0;
                    else
                    {
                       var cant_aggregates = GetAllOrderDetails.Select(c => c.CantAggregates);
                       if(cant_aggregates != null)
                       {
                          if (cant_aggregates.Any())
                          return this.GetAllOrderDetails.Sum(c => c.CantAggregates.Sum(x => x.Quantity));
                          else
                          return 0;
                       }
                       else
                       return 0;
                    }
                }
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
                if (GetAllOrderDetails != null)
                {
                    if (!GetAllOrderDetails.Any())
                    return 0;
                    else
                    {
                      var cant_aggregates = GetAllOrderDetails.Select(c => c.CantAggregates);
                      if (cant_aggregates != null)
                      {
                         if (cant_aggregates.Any())
                         return this.GetAllOrderDetails.Sum(c => c.CantAggregates.Sum(x => x.PriceTotal));
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
                if (GetAllOrderDetails != null)
                {
                    if (!GetAllOrderDetails.Any())
                    return 0;
                    else
                    return GetAllOrderDetails.Sum(c => c.PriceTotal);
                }
                else
                return 0;                  
            }
        }
    }

    /// <summary>
    /// Gps
    /// </summary>
    public class GetGps
    {
        /// <summary>
        /// Key
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Latitud(Coordenada GPS)
        /// </summary>
        public double Latitude_Gps { get; set; }

        /// <summary>
        /// Longitude(Coordenada GPS)
        /// </summary>
        public double Longitude_Gps { get; set; }

        /// <summary>
        /// GPS Favorito
        /// </summary>
        public int Favorite_Gps { get; set; }

        /// <summary>
        /// Nombre del GPS
        /// </summary>
        public string Name_Gps { get; set; }
    }
}
