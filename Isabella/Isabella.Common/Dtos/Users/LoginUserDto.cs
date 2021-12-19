namespace Isabella.Common.Dtos.Users
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Dto para el login del usuario.
    /// </summary>
    public class LoginUserDto
    {
        //Contraseña
        [Required(ErrorMessage = "Introduzca la contraseña.")]
        public string Password { get; set; }

        //Email
        [EmailAddress(ErrorMessage = "El formato del email no es válido.")]
        [MaxLength(1000, ErrorMessage = "Ha superado el limite máximo de caracteres permitidos para el correo electrónico.")]
        public string Email { get; set; }
    }
}
