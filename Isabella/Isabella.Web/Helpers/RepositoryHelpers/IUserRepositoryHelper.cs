namespace Isabella.Web.Helpers.RepositoryHelpers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Models.Entities;

    /// <summary>
    /// Repositorio para los usuarios.
    /// </summary>
    public interface IUserRepositoryHelper
    {
        /// <summary>
        /// Login Web
        /// </summary>
        /// <returns></returns>
        public Task<SignInResult> SignInAsync(User user, string password, bool remember);

        /// <summary>
        /// Logout Web
        /// </summary>
        /// <returns></returns>
        public Task SignOutAsync();

        /// <summary>
        /// Elimina un usuario
        /// </summary>
        /// <returns></returns>
        public Task<bool> DeleteUserAsync(User user);

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
        /// Envia un correo al usuario con los detalles para la confirmación del registro en la aplicación.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<string> GenerateTokenForConfirmRegisterAsync(User user);

        /// <summary>
        /// Obtiene el Id del último usuario registrado.
        /// </summary>
        /// <returns></returns>
        public Task<int> GetIdOfLastUserAsync();

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
        /// Verifica si un role está disponible dado su Id.
        /// </summary>
        /// <param name="RoleId"></param>
        /// <returns></returns>
        public Task<string> VerifyRoleAsync(int RoleId);

        /// <summary>
        /// Verifica si la cuenta de usuario dada está en uso.
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public Task<bool> VerifyUserNameAsync(string userName);

        /// <summary>
        /// Elimina un role especifico de un usuario.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        public Task<bool> RemoveRoleOfUserAsync(User user, string role);

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
        /// Obtiene todos los usuarios del sistema que tienen un role determinado.
        /// </summary>
        /// <param name="RoleId"></param>
        /// <returns></returns>
        public Task<List<User>> GetAllUserWithRoleAsync(int RoleId);

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
        public Task<(DateTime?, string)> CreateTokenAsync(User user);

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
        /// Genera el token del usuario para la recuperación de la contraseña.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<string> GenerateTokenForRecoverPasswordAsync(User user);

        /// <summary>
        /// Recupera la contraseña
        /// </summary>
        /// <param name="user"></param>
        /// <param name="Token"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        public Task<bool> RecoverPasswwordUserAsync(User user, string Token, string newPassword);

        /// <summary>
        /// Verifica si un usuario tiene un role determinado.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        public Task<bool> VerifyRoleInUserAsync(User user, string role);

        /// <summary>
        /// Obtiene todos los roles que posee un usuario.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<List<string>> GetAllRoleOfUserAsync(User user);
    }
}
