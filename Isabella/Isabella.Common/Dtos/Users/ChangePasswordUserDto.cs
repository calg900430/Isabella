namespace Isabella.Common.Dtos.Users
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Dto para cambio de contraseña.
    /// </summary>
    public class ChangePasswordUserDto
    {
        
        //Contraseña Antigua
        [Required(ErrorMessage = "Debe escribir la contraseña actual.")]
        public string PasswordOld { get; set; }

        //Contraseña Nueva
        [Required(ErrorMessage = "Debe escribir la contraseña actual.")]
        [MinLength(8, ErrorMessage = "La contraseña debe tener más de 8 caracteres.")]
        public string PasswordNew { get; set; }

        //Confirmar Contraseña
        [Required(ErrorMessage = "Confirme la contraseña nueva.")]
        [MinLength(8, ErrorMessage = "La contraseña debe tener más de 8 caracteres.")]
        //Compara que la confirmacion sea igual que la contraseña nueva
        [Compare("PasswordNew", ErrorMessage = "Error, no coinciden las contraseñas.")] 
        public string PasswordConfirm { get; set; }
    }
}
