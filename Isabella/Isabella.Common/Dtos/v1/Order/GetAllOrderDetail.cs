namespace Isabella.Common.Dtos.Order
{
    using Isabella.Common.Dtos.CarShop;
    using Isabella.Common.Dtos.Categorie;
    using Isabella.Common.Dtos.SubCategorie;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    public class GetAllOrderDetail
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
                        if (this.CantAggregates.Any())
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
                    if(CantAggregates != null)
                    {
                        if (this.CantAggregates.Any())
                        return (this.Price + this.CantAggregates.Sum(c => c.PriceTotal))
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
}
