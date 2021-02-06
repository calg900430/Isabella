namespace Isabella.Common.Dtos.Users
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class RecoverPasswordDto
    {
        //Contraseña Nueva
        [Required(ErrorMessage = "Escriba la contraseña.")]
        [MinLength(8, ErrorMessage = "La contraseña debe tener más de 8 caracteres.")]
        public string NewPassword { get; set; }

        //Confirmar Contraseña
        [Required(ErrorMessage = "Confirme la contraseña nueva.")]
        [MinLength(8, ErrorMessage = "La contraseña debe tener más de 8 caracteres.")]
        //Compara que la confirmacion sea igual que la contraseña nueva
        [Compare("NewPassword", ErrorMessage = "Error, no coinciden las contraseñas.")]
        public string NewPasswordConfirm { get; set; }

        [Required(ErrorMessage = "Introduzca el código de recuperación.")]
        public string Token { get; set; }
    }
}
