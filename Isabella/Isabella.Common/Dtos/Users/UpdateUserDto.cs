namespace Isabella.Common.Dtos.Users
{
    using System;
    using System.ComponentModel.DataAnnotations;
    
    /// <summary>
    /// Dto para la actualización del usuario.
    /// </summary>
    public class UpdateUserDto
    {
        /// <summary>
        /// Nombre
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Apellidos
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Dirección
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Número de telefono
        /// </summary>
        public int? PhoneNumber { get; set; }

        /// <summary>
        /// Imagen de perfil del usuario.
        /// </summary>
        public byte[] ImageProfile { get; set; }
    }
}
