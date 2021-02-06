namespace Isabella.Common.Dtos.Users
{
    using System;
    using System.Collections.Generic;

    using Extras;

    /// <summary>
    /// User.
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

        /*/// <summary>
        /// Devuelve la URL completa donde se encuentran la imagen del usuario(Para apk)
        /// </summary>
        public string ImageFullPath { get; set; }

        /// <summary>
        /// Guarda la URL donde se encuentran la imagen del usuario
        /// pero partir del caracter virgulilla.
        /// </summary>
        public string ImageUserPath { get; set; }*/
    }
}
