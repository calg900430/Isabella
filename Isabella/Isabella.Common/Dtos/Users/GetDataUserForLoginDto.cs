namespace Isabella.Common.Dtos.Users
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// User.
    /// </summary>
    public class GetDataUserForLoginDto
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

        /// <summary>
        /// Fecha de expiración de Token.
        /// </summary>
        public DateTime? DateExpirationToken { get; set; }

        /// <summary>
        /// Token Bearer
        /// </summary>
        public string Token { get; set; }
        
        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Role del usuario.
        /// </summary>
        public List<string> RolesOfUsers { get; set; }
    }
}
