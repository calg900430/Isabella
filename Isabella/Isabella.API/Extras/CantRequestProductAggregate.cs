namespace Isabella.API.Extras
{
    using Isabella.API.Models;

    /// <summary>
    /// Representa un producto agregado y la cantidad deseada.
    /// </summary>
    public class CantRequestProductAggregate
    {
        /// <summary>
        /// ProductoStandard.
        /// </summary>
        public ProductAggregate ProductAggregate { get; set; }

        /// <summary>
        /// Cantidad que desea.
        /// </summary>
        public int Quantity { get; set; }

       
    }
}
