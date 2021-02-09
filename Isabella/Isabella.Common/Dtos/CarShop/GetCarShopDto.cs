namespace Isabella.Common.Dtos.CarShop
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Category;
    using System.Linq;

    /// <summary>
    /// Productos en el carrito para pedido un futuro pedido.
    /// </summary>
    public class GetCarShopDto
    {
        public Guid Verification { get; set; }

        /// <summary>
        /// Productos Standards
        /// </summary>
        public List<GetCarShopProductStandard> CarShopProductStandards { get; set; }

        /// <summary>
        /// Productos Specials
        /// </summary>
        public List<GetCarShopProductSpecial> CarShopProductSpecial { get; set; }

        /// <summary>
        /// Cantidad de productos sin repetir(o sea productos diferentes) que tiene nuestro carrito.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int Lines
        {
            get
            {
                int cantProductSpecial = 0;
                int cantProductStandard = 0;
                if (CarShopProductSpecial != null)
                cantProductSpecial = this.CarShopProductSpecial.Count;
                if (CarShopProductStandards != null)
                cantProductStandard = this.CarShopProductStandards.Count;
                return cantProductSpecial + cantProductStandard;
            }
        }

        /// <summary>
        /// Cantidad Total de productos en el carrito.
        /// </summary>
        public int QuantityTotal 
        {
            get
            {
                int QuantityProductSpecial = 0;
                int QuantityProductStandard = 0;
                if (CarShopProductSpecial != null)
                QuantityProductSpecial = this.CarShopProductSpecial.Sum(c => c.Quantity);
                if (CarShopProductStandards != null)
                QuantityProductStandard = this.CarShopProductStandards.Sum(c => c.Quantity);
                return QuantityProductStandard + QuantityProductSpecial;
            }

        }

        /// <summary>
        /// Precio Total del posible pedido.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal PriceTotal 
        { 
            get 
            {
                decimal price_total_products_standard = 0;
                decimal price_total_products_special = 0;
                if (CarShopProductStandards != null)
                price_total_products_standard = CarShopProductStandards.Sum(c => c.PriceTotal);
                if(CarShopProductSpecial != null)
                price_total_products_special = CarShopProductSpecial.Sum(c => c.PriceTotal);
                return price_total_products_standard + price_total_products_special;
            } 
        }
    }

    /// <summary>
    /// Pedidos de productos standards
    /// </summary>
    public class GetCarShopProductStandard
    {

        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nombre del Producto ofertado por el Restaurante.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Categoria
        /// </summary>
        public GetCategoryDto Category { get; set; }

        /// <summary>
        /// Precio del Produto.
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Promedio de calificaciones del usuario acerca del producto.
        /// </summary>
        public float Average { get; set; }

        /// <summary>
        /// Cantidad deseada.
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Precio Total del pedido de este producto.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal PriceTotal { get { return this.Price * (decimal)this.Quantity; } }
    }

    /// <summary>
    /// Pedidos de productos standards
    /// </summary>
    public class GetCarShopProductSpecial
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Queso Gouda
        /// </summary>
        public bool CheeseGouda { get; set; }

        /// <summary>
        /// Categoria
        /// </summary>
        public GetCategoryDto Category { get; set; }

        /// <summary>
        /// Cantidad de agregos, solicitados para el producto.
        /// </summary>
        public List<GetCarShopProductAggregate> GetCarShopProductAggregates { get; set; }

        /// <summary>
        /// Nombre del Producto ofertado por el Restaurante.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Precio del Produto.
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Promedio de calificaciones del usuario acerca del producto.
        /// </summary>
        public float Average { get; set; }

        /// <summary>
        /// Precio Total de los productos de agregado.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal PriceTotalProductAggregate
        {
            get
            {
                decimal precio_total_agregados = 0;
                if (GetCarShopProductAggregates != null)
                precio_total_agregados = GetCarShopProductAggregates.Sum(c => c.PriceTotal);
                return precio_total_agregados;
            }
        }

        /// <summary>
        /// Cantidad Total de los productos de agregado.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public int QuantityTotalProductAggregate
        {
            get
            {
                int quantity = 0;
                if (GetCarShopProductAggregates != null)
                quantity = GetCarShopProductAggregates.Sum(c => c.Quantity);
                return quantity;
            }
        }

        /// <summary>
        /// Cantidad de Productos.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public int Quantity { get; set; }

        /// <summary>
        /// Precio total del Producto.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal PriceTotal
        {
            get
            {
                //El pedido del producto espcial es con queso Gouda
                decimal price_gouda = 0;
                if (CheeseGouda)
                price_gouda = 35;
                return (this.Price * (decimal)this.Quantity) + this.PriceTotalProductAggregate + price_gouda;
            }
        }
    }

    public class GetCarShopProductAggregate
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }

        //Id del producto que incluye el agrego.
        public int CarShopProductSpecialId { get; set; }

        /// <summary>
        /// Categoria
        /// </summary>
        public GetCategoryDto Category { get; set; }

        /// <summary>
        /// Nombre del Producto ofertado por el Restaurante.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Precio del Produto.
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Cantidad deseada.
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Precio Total del pedido de este producto.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal PriceTotal { get { return this.Price * (decimal)this.Quantity; } }
    }
}
