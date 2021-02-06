namespace Isabella.Common.Dtos.Users
{
    using System.ComponentModel.DataAnnotations;

    public class ConfirmEmailDto
    {
        //Email
        [Required(ErrorMessage = "Debe introducir el correo del usuario.")]
        [EmailAddress(ErrorMessage = "El formato del email no es válido.")]
        [MaxLength(1000, ErrorMessage = "Ha superado el limite máximo de caracteres permitidos para el correo electrónico.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Introduzca el código de confirmación.")]
        [MaxLength(1000, ErrorMessage = "Ha superado el limite máximo de caracteres permitidos para el código de verificación.")]
        public string Token { get; set; }
    }
}
