namespace Isabella.Common.Dtos.Users
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// User
    /// </summary>
    public class GetUserDto
    {
        /// <summary>
        /// Key
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        ///  Nombre del usuario
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Apellidos del usuario
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Telefono Celular
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Dirección
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Imagen de perfil del usuario.
        /// </summary>
        public byte[] ImageUserProfile { get; set; }
    }
}
