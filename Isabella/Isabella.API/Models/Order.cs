namespace Isabella.API.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    /// <summary>
    /// Ordenes del tipo fast(Para pedidos informales, o sea usuarios que no desean ser clientes oficiales)
    /// </summary>
    public class Order
    {
        /// <summary>
        /// Key
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Gps
        /// </summary>
        public Gps Gps { get; set; }

        /// <summary>
        /// Dirección donde entregar la orden
        /// </summary>
        [Required]
        public string Address { get; set; }

        /// <summary>
        /// Detalles de cada producto que tiene la orden.
        /// </summary>
        public ICollection<OrderDetail> Items { get; set; }

        /// <summary>
        /// Cantidad de productos sin repetir(o sea productos diferentes) que tiene nuestro pedido.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int Lines 
        {  
           get
           {
                int cantProductSpecial = 0;
                int cantProductStandard = 0;
                var AllProductSpecial = this.Items.Select(c => c.Requested_ProductSpecial).ToList();
                if(AllProductSpecial != null)
                cantProductSpecial = AllProductSpecial.Count;
                var AllProductStandard = this.Items.Select(c => c.RequestedProductStandard).ToList();
                if(AllProductStandard != null)
                cantProductStandard = AllProductStandard.Count;
                return cantProductSpecial + cantProductStandard;
           } 
        }

        /// <summary>
        /// Cantidad Total de todos los productos de la orden
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public double Quantity 
        { 
            get 
            {

                //return this.Items == null ? 0 : this.Items.Sum(i => i.Quantity); 
                return 0;
            } 
        }

        /// <summary>
        /// Precio Total de la Orden
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal Value 
        { 
            get
            {
                //return this.Items == null ? 0 : this.Items.Sum(i => i.Value); 
                return 0;
            }
        }

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
    }
}
