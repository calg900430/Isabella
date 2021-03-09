namespace Isabella.API.Models.Entities
{
    using System;
    using Models;

    /// <summary>
    /// Código para indentificar el pedido.(No hay usuarios)
    /// </summary>
    public class CodeIdentification : IEntity
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
