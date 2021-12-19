namespace Isabella.Web.Models.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Microsoft.AspNetCore.Identity;

    using Models;
   
    /// <summary>
    /// Users.
    /// </summary>
    public class User : IdentityUser<int>, IEntity
    {
	    /// <summary>
        /// Id para Claim
        /// </summary>
	    public Guid IdForClaim {get; set;}
        
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
        /// Dirección.
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Indica que el usuario solicito recuperación de contraseña.
        /// </summary>
        public bool RecoverPassword { get; set; }

        /// <summary>
        /// Enable
        /// </summary>
        public bool Enable { get; set; }

        /// <summary>
        /// Imagen de perfil del usuario.
        /// </summary>
        public byte[] ImageUserProfile { get; set; }


        /*/// <summary>
        /// Guarda la URL donde se encuentran la imagen del usuario
        /// pero partir del caracter virgulilla.
        /// </summary>
        public string ImageUserPath { get; set; }

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
                return $"http:/ndbcorporation-001-site1.ftempurl.com/{this.ImageUserPath.Substring(1)}";
            }
        }*/
    }
}
