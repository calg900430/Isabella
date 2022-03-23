namespace Isabella.Web.ViewModels.UsersViewModel
{
    using Isabella.Common.Dtos.Users;

    /// <summary>
    /// Login Web de usuario
    /// </summary>
    public class LoginViewModel : LoginUserDto
    {
        /// <summary>
        /// Recordar usuario
        /// </summary>
        public bool RememberMe { get; set; }
    }
}
