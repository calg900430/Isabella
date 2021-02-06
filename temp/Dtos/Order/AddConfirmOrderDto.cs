namespace Isabella.Common.Dtos.Order
{
    using Isabella.Common.Extras;
    using System.ComponentModel.DataAnnotations;
    public class AddConfirmOrderDto
    {
        /// <summary>
        /// Código de usuario
        /// </summary>
        [Required(ErrorMessage = "Debe introducir su código de usuario.")]
        [MaxLength(20, ErrorMessage = "Ha superado el limite máximo de caracteres permitidos.")]
        public string CodeUser { get; set; }

        /// <summary>
        /// Coordenadas GPS
        /// </summary>
        public Gps Gps { get; set; }

        /// <summary>
        /// Dirección donde se debe entregar el pedido
        /// </summary>
        [Required(ErrorMessage = "Debe introducir una dirección de entrega del pedido.")]
        public string Address { get; set; }
    }
}
