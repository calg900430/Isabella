using System;
using System.Collections.Generic;
using System.Text;


namespace Isabella.Common.Dtos.Resturant
{
    public class UpdateRestuarantDto
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nombre
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Indica si el restaurante está abierto o cerrado.
        /// </summary>
        public bool IsOpenRestaurant { get; set; }

        /// <summary>
        /// Hora de apertura del restaurante
        /// </summary>
        public DateTime BeginHour { get; set; }

        /// <summary>
        /// Horra de cerrar el restaurante
        /// </summary>
        public DateTime CloseHour { get; set; }
    }
}
