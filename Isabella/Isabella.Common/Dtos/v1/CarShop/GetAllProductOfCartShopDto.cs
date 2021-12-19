namespace Isabella.Common.Dtos.CarShop
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using Categorie;
    using SubCategorie;
    using Isabella.Common.Dtos.Users;

    /// <summary>
    /// Productos en el carrito para pedido un futuro pedido.
    /// </summary>
    public class GetAllProductOfCartShopDto
    {
        /// <summary>
        /// User
        /// </summary>
        public GetUserDto GetUserDto { get; set; }

        /// <summary>
        /// Productos
        /// </summary>
        public List<GetCarShopProductDto> GetCarShopProducts { get; set; }

        /// <summary>
        /// Cantidad de productos sin repetir(o sea productos diferentes) que tiene nuestro carrito.
        /// </summary>
        public int Lines
        {
            get
            {
                return GetCarShopProducts.Count;
            }
        }

        /// <summary>
        /// Cantidad Total de Productos.
        /// </summary>
        public int QuantityTotalProductCombined
        {
            get
            {
                if(GetCarShopProducts != null)
                {
                   if (!GetCarShopProducts.Any())
                   return 0;
                   else
                   return this.GetCarShopProducts.Sum(c => c.Quantity);
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
                if (GetCarShopProducts != null)
                {
                    if (!GetCarShopProducts.Any())
                    return 0;
                    else
                    {
                        var cant_aggregates = GetCarShopProducts.Select(c => c.CantAggregates);
                        if(cant_aggregates != null)
                        {
                           if (cant_aggregates.Any())
                           return this.GetCarShopProducts.Sum(c => c.CantAggregates.Sum(x => x.Quantity));
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
        /// Precio total en agregados
        /// </summary>
        public decimal PriceTotalOfAggregates
        {
            get
            {
                if(GetCarShopProducts != null)
                {
                    if (!GetCarShopProducts.Any())
                    return 0;
                    else
                    {
                       var cant_aggregates = GetCarShopProducts.Select(c => c.CantAggregates);
                       if(cant_aggregates != null)
                       { 
                          if (cant_aggregates.Any())
                          return this.GetCarShopProducts.Sum(c => c.CantAggregates.Sum(x => x.PriceTotal));
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
        /// Precio Total del posible pedido.
        /// </summary>
        public decimal PriceTotal
        {
            get
            {
                if (GetCarShopProducts != null)
                {
                    if (!GetCarShopProducts.Any())
                    return 0;
                    else
                    return GetCarShopProducts.Sum(c => c.PriceTotal);
                }
                else
                return 0;
            }
        }

    }

    /// <summary>
    /// Pedidos de productos standards
    /// </summary>
    public class GetCarShopProductDto
    {
        /// <summary>
        /// Id del producto combinado, o sea el que ya está en el carrito
        /// personalizado al gusto del usuario.
        /// </summary>
        public int ProductCombinedId { get; set; }

        /// <summary>
        /// Producto
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// Nombre del Producto ofertado por el Restaurante.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Precio del Produto.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal Price { get; set; }

        /// <summary>
        /// Descripción del Producto.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Promedio de calificaciones del usuario acerca del producto.
        /// </summary>
        public float Average { get; set; }

        /// <summary>
        /// Indica si el producto está disponible.
        /// </summary>
        public bool IsAvailabe { get; set; }

        /// <summary>
        /// Indica si el producto se le puede incluir agregados.
        /// </summary>
        public bool SupportAggregate { get; set; }

        /// <summary>
        /// Categoria del producto.
        /// </summary>
        public GetCategorieDto Category { get; set; }

        /// <summary>
        /// SubCategoria
        /// </summary>
        public GetSubCategorieDto SubCategory { get; set; }

        /// <summary>
        /// Agregados.
        /// </summary>
        public List<GetCantAggregateDto> CantAggregates { get; set; }

        /// <summary>
        /// Cantidad de Productos.
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Precio total del producto, incluye agregados y en caso de pizzas y pastas si es con queso gouda..
        /// </summary>
        public decimal PriceTotal 
        { 
            get
            {
                if (this.SubCategory != null)
                {
                   if(CantAggregates != null)
                   {
                      if(CantAggregates.Any())
                      return (this.SubCategory.Price +
                      this.CantAggregates.Sum(c => c.PriceTotal))
                      * (this.Quantity);
                      else
                      return this.SubCategory.Price * this.Quantity;
                   }
                   else
                   return this.SubCategory.Price * this.Quantity;
                }
                else
                {
                    if (CantAggregates != null)
                    {
                       if (this.CantAggregates.Any())
                       return (this.Price +
                       this.CantAggregates.Sum(c => c.PriceTotal))
                       * (this.Quantity);
                       else
                       return this.Price * this.Quantity;
                    }
                    else
                    return this.Price * this.Quantity;
                }
            }
        }

        /// <summary>
        /// Fecha en que se agrego el producto al carrito.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? DateCreated { get; set; }
    }

    public class GetCantAggregateDto
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
        /// Precio del Produto.
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Cantidad deseada.
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Precio total del pedido de este agrego.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal PriceTotal
        {
            get
            {
                return this.Price * this.Quantity;
            }
        }
    }
}
