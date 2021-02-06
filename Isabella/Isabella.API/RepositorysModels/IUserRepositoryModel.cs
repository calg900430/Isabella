namespace Isabella.API.RepositorysModels
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;

    using Extras;
    using Models;
   
    /// <summary>
    /// Repositorio para los usuarios.
    /// </summary>
    public interface IUserRepositoryModel
    {
        /// <summary>
        /// Agrega un usuario con contraseña definida.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public Task<User> AddUserAsync(User user, string password);

        /// <summary>
        /// Agrega un usuario sin contraseña definida(Esto es para login con proveedor externo).
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<User> AddUserAsync(User user);

        /// <summary>
        /// Envia un correo al usuario con el código para confirmación del registro en la aplicación.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<bool> SendEmailWithCodeConfirmRegisterAsync(User user);

        /// <summary>
        /// Verifica si existe el role en el sistema.
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public Task<bool> VerifyRoleAsync(string role);

        /// <summary>
        /// Verifica si el email dado está en uso.
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public Task<bool> VerifyEmailAsync(string email);

        /// <summary>
        /// Verifica si la cuenta de usuario dada está en uso.
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public Task<bool> VerifyUserNameAsync(string userName);

        /// <summary>
        /// Obtiene el Id del último usuario registrado en el sistema.
        /// </summary>
        /// <returns></returns>
        public Task<int> GetIdOfLastUserAsync();

        /// <summary>
        /// Obtiene un usuario dada su cuenta de usuario.
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public Task<User> GetUserByUserNameAsync(string userName);

        /// <summary>
        /// Obtiene un Usuario dado su Id.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Task<User> GetUserByIdAsync(int userId);

        /// <summary>
        /// Obtiene un usuario dado su correo electrónico.
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public Task<User> GetUserByEmailAsync(string email);

        /// <summary>
        /// Obtiene todos los usuarios del sistema.
        /// </summary>
        /// <returns></returns>
        public Task<List<User>> GetAllUserAsync();

        /// <summary>
        /// Agregar un role a un usuario.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        public Task<bool> AddRoleForUserAsync(User user, string role);

        /// <summary>
        /// Verifica si el usuario confirmó su registro.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<bool> VerifyConfirmRegisterUserAsync(User user);

        /// <summary>
        /// Confirma el registro del usuario confirmó su registro.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="Token"></param>
        /// <returns></returns>
        public Task<bool> ConfirmRegisterUserAsync(User user, string Token);

        /// <summary>
        /// Verifica que la contraseña del usuario es correcta.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public Task<bool> VerifyPasswordUserAsync(User user, string password);

        /// <summary>
        /// Crea un Token Web Json para el usuario.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<Token_DateTimeExpired_Roles> CreateTokenAsync(User user);

        /// <summary>
        /// Borra la imagen de perfil de un usuario determinado.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<bool> DeleteImageProfileUserAsync(User user);

        /// <summary>
        /// Cambia la contraseña de un usuario.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="oldPassword"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        public Task<bool> UpdatePasswordAsync(User user, string oldPassword, string newPassword);

        /// <summary>
        /// Actualiza el usuario.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<User> UpdateUserAsync(User user);

        /// <summary>
        /// Envia un correo al usuario con un código para la recuperaión de su contraseña.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<bool> SendEmailForResetPasswordUserAsync(User user);

        /// <summary>
        /// Verifica si el usuario solicito la recuperación de su contraseña.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool VerifyUserRecoverPasswordAsync(User user);

        /// <summary>
        /// Recupera la contraseña
        /// </summary>
        /// <param name="user"></param>
        /// <param name="Token"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        public Task<bool> RecoverPasswwordUserAsync(User user, string Token, string newPassword);

        /// <summary>
        /// Agrega o actualiza la imagen de perfil del usuario usando la IFormFile.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="formFile"></param>
        /// <returns></returns>
        public Task<bool> AddOrUpdateImageProfileUserAsync(User user, IFormFile formFile);
    }
}
