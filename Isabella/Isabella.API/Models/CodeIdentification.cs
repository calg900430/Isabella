namespace Isabella.API.Models
{
   
    using System;
    using Extras;

    /// <summary>
    /// Código para indentificar el pedido.(No hay usuarios)
    /// </summary>
    public class CodeIdentification : IModel
    {
        /// <summary>
        /// 
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Código para el pedido en modo Fast.
        /// </summary>
        public Guid Code { get; set; }   
    }
}
