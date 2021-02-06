namespace Isabella.Common.Dtos.Order
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using Extras;
    using Product;

    public class AddProduct_PizzaPasta_ToCarShopDto
    {
        /// <summary>
        /// Código de usuario
        /// </summary>
        [Required(ErrorMessage = "Debe introducir su código de usuario.")]
        public string CodeUser { get; set; }

        /// <summary>
        /// ProductId
        /// </summary>
        [Required(ErrorMessage = "Debe especificar el Id del producto.")]
        public int Product_Pizza_PastaId { get; set; }

        /// <summary>
        /// Cantidad de agregados con lo que el usuario quiere el producto.
        /// </summary>
        public HashSet<AddTypeAddFor_PizzaPastaDto> CantProductForAdds { get; set; } 
        
        /// <summary>
        /// Tipo de queso, solo para Pizzas y Pastas.
        /// </summary>
        public EnumTypeCheeses TypeCheeses { get; set; }

        /// <summary>
        /// Cantidad de Productos que piensa adquirir el usuario
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:N2}")]
        [Required(ErrorMessage = "Debe introducir la cantidad de productos que desea.")]
        [Range(1,int.MaxValue, ErrorMessage = "La cantidad de productos seleccionada está fuera del rango permitido.")]
        public int Quantity { get; set; }     
    }
}
