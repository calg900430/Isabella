namespace Isabella.Common.Dtos.Users
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Obtiene todos los datos de un usuario.
    /// </summary>
    public class GetUserAllDataForOnlyAdmin
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
        /// Email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Role del usuario.
        /// </summary>
        public List<string> RolesOfUsers { get; set; }

        /// <summary>
        /// Id para Claim
        /// </summary>
        public Guid IdForClaim { get; set; }

        /// <summary>
        /// Fecha en la que se actualizó campos del usuario en el sistema.
        /// </summary>
        public DateTime? DateCreated { get; set; }

        /// <summary>
        /// Fecha en el que se registró el usuario en el sistema.
        /// </summary
        public DateTime? DateUpdated { get; set; }

        /// <summary>
        /// Ultima vez que el usuario se conecto a la API InkNation.
        /// </summary>
        public DateTime? LastDateConnected { get; set; }

    }
}
