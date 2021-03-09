namespace Isabella.Common.Dtos.Order
{
    using Isabella.Common.Dtos.CarShop;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    public class GetAllOrderDto
    {
       
        /// <summary>
        /// Código de identificación.
        /// </summary>
        public Guid CodeVerification { get; set; }

        /// <summary>
        /// Ordenes
        /// </summary>
        public List<GetAllOrder> GetAllOrders { get; set; }

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
                if (!GetAllOrders.Any())
                return 0;
                else
                return this.GetAllOrders.Sum(c => c.QuantityTotalProductCombined);
            }

        }

        /// <summary>
        /// Cantidad Total de Agregados.
        /// </summary>
        public int QuantityTotalAggregateOfOrder
        {
            get
            {
                if (!GetAllOrders.Any())
                return 0;
                else
                return this.GetAllOrders.Sum(c => c.QuantityTotalAggregate);
            }
        }

        /// <summary>
        /// Precio total del de productos
        /// </summary>
        public decimal PriceTotalOfProductCombinedOfOrder
        {
            get
            {
                if (!GetAllOrders.Any())
                return 0;
                else
                return this.GetAllOrders.Sum(c => c.PriceTotal);
            }
        }

        /// <summary>
        /// Precio total en agregados
        /// </summary>
        public decimal PriceTotalOfAggregatesOfOrder
        {
            get
            {
                if (!GetAllOrders.Any())
                return 0;
                else
                return this.GetAllOrders.Sum(c => c.PriceTotalOfAggregates);
            }
        }

        /// <summary>
        /// Precio Total del posible pedido.
        /// </summary>
        public decimal PriceTotalOfOrder
        {
            get
            {
                if (!GetAllOrders.Any())
                    return 0;
                else
                    return this.GetAllOrders.Sum(c => c.PriceTotal);
            }
        }

    }

    public class GetAllOrder
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
                if (!GetAllOrderDetails.Any())
                return 0;
                else
                return this.GetAllOrderDetails.Sum(c => c.Quantity);
            }

        }

        /// <summary>
        /// Cantidad Total de Agregados.
        /// </summary>
        public int QuantityTotalAggregate
        {
            get
            {
                if (!GetAllOrderDetails.Any())
                return 0;
                else
                return this.GetAllOrderDetails.Sum(c => c.CantAggregates.Sum(x => x.Quantity));
            }
        }

        /// <summary>
        /// Precio total del de productos
        /// </summary>
        public decimal PriceTotalOfProductCombined
        {
            get
            {
                if (!GetAllOrderDetails.Any())
                    return 0;
                else
                    return this.GetAllOrderDetails.Sum(c => c.PriceTotal);
            }
        }

        /// <summary>
        /// Precio total en agregados
        /// </summary>
        public decimal PriceTotalOfAggregates
        {
            get
            {
                if (!GetAllOrderDetails.Any())
                    return 0;
                else
                    return this.GetAllOrderDetails.Sum(c => c.CantAggregates.Sum(x => x.PriceTotal));
            }
        }

        /// <summary>
        /// Precio Total del posible pedido.
        /// </summary>
        public decimal PriceTotal
        {
            get
            {
                if (!GetAllOrderDetails.Any())
                return 0;
                else
                return PriceTotalOfProductCombined + PriceTotalOfAggregates;
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
