namespace Isabella.Common.Dtos.Users
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class ResetPasswordDto
    {
        //Email
        [Required(ErrorMessage = "Debe introducir el correo del usuario.")]
        [EmailAddress(ErrorMessage = "El formato del email no es válido.")]
        [MaxLength(1000, ErrorMessage = "Ha superado el limite máximo de caracteres permitidos para el correo electrónico.")]
        public string Email { get; set; }

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

    }
}
