namespace Isabella.Common.Dtos.Users
{
    using System;
    using System.Collections.Generic;

    using Common.Extras;
    /// <summary>
    /// Respuesta para el login de usuarios por la APIRESTful
    /// </summary>
    public class ResponseLoginTokenDto
    {
        /// <summary>
        /// Token de acceso.
        /// </summary>
        public string Token { get; set;}

        /// <summary>
        /// Fecha de Expiración.
        /// </summary>
        public DateTime? DateExpiration { get; set; }

        /// <summary>
        ///  Email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Cuenta de usuario
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Roles del usuario.
        /// </summary>
        public List<EnumRoles> Roles { get; set; }

        /// <summary>
        /// Datos del usuario.
        /// </summary>
        public GetUserDto GetUser { get; set; }
    }
}
