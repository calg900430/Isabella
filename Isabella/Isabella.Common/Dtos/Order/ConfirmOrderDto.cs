namespace Isabella.Common.Dtos.Order
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class ConfirmOrderDto
    {
       
        /// <summary>
        /// Gps
        /// </summary>
        public AddGps AddGps { get; set; }

        /// <summary>
        /// Gps
        /// </summary>
        [Phone(ErrorMessage = "El formato del número de telefono no es valido.")]
        [Required(ErrorMessage = "Debe insertar un número de telefono para confirmar el pedido.")]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Dirección donde entregar la orden
        /// </summary>
        [Required(ErrorMessage = "Debe definir la dirección de entrega del pedido.")]
        public string Address { get; set; }

        /// <summary>
        /// Dirección donde entregar la orden
        /// </summary>
        [Required(ErrorMessage = "Debe definir un nombre o un alias de quién solicitó el pedido.")]
        public string AskForWho { get; set; }

    }

    public class AddGps 
    {
        /// <summary>
        /// Latitud(Coordenada GPS)
        /// </summary>
        public double Latitude_Gps { get; set; }

        /// <summary>
        /// Longitude(Coordenada GPS)
        /// </summary>
        public double Longitude_Gps { get; set; }

        /// <summary>
        /// Nombre del GPS
        /// </summary>
        public string Name_Gps { get; set; }
    }
}
