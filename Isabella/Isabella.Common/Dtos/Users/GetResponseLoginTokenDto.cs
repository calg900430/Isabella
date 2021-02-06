namespace Isabella.Common.Dtos.Users
{
    using System;
    using System.Collections.Generic;
    using Extras;
  
    /// <summary>
    /// Respuesta para el login de usuarios por la APIRESTful
    /// </summary>
    public class GetResponseLoginTokenDto
    {
        /// <summary>
        /// Token de acceso.
        /// </summary>
        public string Token { get; set;}

        /// <summary>
        /// Rol del usuario.
        /// </summary>
        public List<EnumRoles> UserRoles { get; set; }

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
        /// Datos del usuario.
        /// </summary>
        public GetUserDto GetUser { get; set; }
    }
}
