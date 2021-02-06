namespace Isabella.Web.Extras
{
    using System;
    using System.Collections.Generic;
    using Common.Extras;

    /// <summary>
    /// Representa el Token y la fecha de expiración del mismo
    /// </summary>
    public class Token_DateTimeExpired_Roles
    {
        /// <summary>
        /// Token
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// DateTime Expired
        /// </summary>
        public DateTime? DateTime { get; set; }

        /// <summary>
        /// Roles
        /// </summary>
        public List<EnumRoles> UserRoles { get; set; }
    }
}
