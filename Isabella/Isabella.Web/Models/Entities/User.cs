namespace Isabella.Web.Models.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Microsoft.AspNetCore.Identity;

    using Common.Extras;
    using Extras;
   
    /// <summary>
    /// Representa la entidad de los Usuarios en general.
    /// </summary>
    public class User : IdentityUser<int>
    {
        /// <summary>
        /// Codigo de usuario.
        /// </summary>
        [Display(Name = "Code User")]
        public Guid CodeUser { get; set; }

        /// <summary>
        /// Fecha en la que se actualizó campos del usuario en el sistema.
        /// </summary>
        [Display(Name = "Date Created")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? DateCreated { get; set; }

        /// <summary>
        /// Fecha en el que se registró el usuario en el sistema.
        /// </summary>
        [Display(Name = "Date Updated")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? DateUpdated { get; set; }

        /// <summary>
        /// Ultima vez que el usuario se conecto a la API InkNation.
        /// </summary>
        [Display(Name = "Last Connected")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? LastDateConnected { get; set; }

        /// <summary>
        /// Nombre
        /// </summary>
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        /// <summary>
        /// Apellidos
        /// </summary>
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        /// <summary>
        /// Nombre completo del usuario.
        /// </summary>
        public string FullName { get => $"{FirstName} { LastName}"; }

        /// <summary>
        /// Guarda la URL donde se encuentran la imagen del usuario
        /// pero partir del caracter virgulilla.
        /// </summary>
        public string ImageUserPath { get; set; }

        /// <summary>
        /// Dirección.
        /// </summary>
        [Display(Name = "Address")]
        public string Address { get; set; }

        /// <summary>
        /// Devuelve la URL completa donde se encuentran la imagen del usuario(Para apk)
        /// </summary>
        public string ImageFullPath
        {
            get
            {
                if (string.IsNullOrEmpty(this.ImageUserPath))
                {
                    return null;
                }
                //Elimina el caracter virgulilla y devuelve la ruta completa de donde se encuentra la imagen.
                return $"https://inknation.azurewebsites.net{this.ImageUserPath.Substring(1)}";
            }
        }

        /// <summary>
        /// Rol del usuario.
        /// </summary>
        public EnumRoles Role { get; set; }

        /// <summary>
        /// Indica que el usuario solicito recuperación de contraseña.
        /// </summary>
        public bool RecoverPassword { get; set; }

    }
}
