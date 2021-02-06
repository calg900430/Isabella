namespace Duma.API.Repositorys
{
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;

    using Common;
    using Common.Dtos.Users;
    using Common.Extras;
    using Extras;
    using Models;
   
    /// <summary>
    /// Repositorio para el manejo de los usuarios.
    /// </summary>
    public interface IUserRepository
    {

        /// <summary>
        /// Obtiene todos los usuarios del sistema.
        /// </summary>
        /// <returns></returns>
        public Task<ServiceResponse<List<GetUserDto>>> GetAllUserAsync();

        /// <summary>
        /// Agrega un usuario en la Base de Datos a través de login por proveedor externo(Google, Facebook,Apple,ect).
        /// </summary>
        /// <param name="Email"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        Task<User> AddUserAsyncForExternProvider(string Email, string role);

        /// <summary>
        /// Crea un Token JWT para el usuario.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task<Token_DateTimeExpired_Roles> CreateTokenAsync(User user);

        /// <summary>
        /// Agrega un usuario en la Base de Datos.
        /// </summary>
        /// <param name="newuser"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        public Task<ServiceResponse<bool>> AddUserAsync(RegisterUserDto newuser, string role);

        /// <summary>
        /// Obtiene el Id del último usuario en la base de datos.
        /// </summary>
        /// <returns></returns>
        Task<ServiceResponse<int>> GetIdOfLastUser();

        /// <summary>
        /// Verifica si el usuario tiene un role determinado
        /// </summary>
        /// <param name="user"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        Task<ServiceResponse<bool>> VerifyUserIfRoleBarber(User user, string role); 

        /// <summary>
        /// Obtiene un Usuario según su cuenta de usuario.
        /// </summary>
        /// <param name="UserName"></param>
        /// <returns></returns>
        Task<ServiceResponse<GetUserDto>> GetUserByUserNameAsync(string UserName);

        /// <summary>
        /// Obtiene un Usuario según su Id Int.
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public Task<ServiceResponse<GetUserDto>> GetUserByIdAsync(int UserId);

        /// <summary>
        /// Obtiene un usuario según su correo.
        /// </summary>
        /// <param name="Email"></param>
        /// <returns></returns>
        public Task<ServiceResponse<GetUserDto>> GetUserByEmailAsync(string Email);

        /// <summary>
        /// Obtiene un usuario dado su CodeUser.
        /// </summary>
        /// <param name="CodeUser"></param>
        /// <returns></returns>
        public Task<ServiceResponse<User>> GetUserByCodeUserAsync(string CodeUser);

        /// <summary>
        /// Devuelve una lista de usuarios a partir de uno de referencia.
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="CantUsers"></param>
        /// <returns></returns>
        public Task<ServiceResponse<List<GetUserDto>>> GetCantUserAsync(int UserId, int CantUsers);

        /// <summary>
        /// Cambia la contraseña del usuario.
        /// </summary>
        /// <param name="changePassword"></param>
        /// <returns></returns>
        Task<ServiceResponse<IdentityResult>> UpdatePasswordAsync(ChangePasswordUserDto changePassword);

        /// <summary>
        /// Actualiza todos los campos que desee el usuario menos el correo.
        /// </summary>
        /// <param name="updateUser"></param>
        /// <returns></returns>
        Task<ServiceResponse<GetUserDto>> UpdateUserAsync(UpdateUserDto updateUser);

        /// <summary>
        /// Login del usuario.
        /// </summary>
        /// <param name="loginUser"></param>
        /// <returns></returns>
        Task<ServiceResponse<ResponseLoginTokenDto>> LoginUserForApiAsync(LoginUserWithUserNameDto loginUser);

        /// <summary>
        /// Confirma el registro del usuario.
        /// </summary>
        /// <param name="confirmEmail"></param>
        /// <returns></returns>
        Task<ServiceResponse<bool>> ConfirmEmailUserAsync(ConfirmEmailDto confirmEmail);

        /// <summary>
        /// Envia un correo al usuario con los detalles para la recuperaión de su contraseña.
        /// </summary>
        /// <param name="resetPassword"></param>
        /// <returns></returns>
        Task<ServiceResponse<bool>> ResetPasswordUserAsync(ResetPasswordDto resetPassword);

        /// <summary>
        /// Recupera la contraseña del usuario contraseña del usuario. 
        /// </summary>
        /// <param name="recoverPassword"></param>
        /// <returns></returns>
        Task<ServiceResponse<bool>> RecoverPasswwordUserAsync(RecoverPasswordDto recoverPassword);

        /// <summary>
        /// Envia un nuevo código para la confirmación del registro.
        /// </summary>
        /// <param name="sendToNewCodeConfirmationRegister"></param>
        /// <returns></returns>
        Task<ServiceResponse<bool>> SendToNewCodeConfirmationEmail(SendToNewCodeConfirmationRegisterDto sendToNewCodeConfirmationRegister);

        /// <summary>
        /// Agrega una imagen de perfil de un usuario determinado
        /// </summary>
        /// <param name="formFile"></param>
        /// <param name="CodeUser"></param>
        /// <returns></returns>
        public Task<ServiceResponse<bool>> AddImageProfileUserAsync(IFormFile formFile, string CodeUser);

        /// <summary>
        /// Borra la imagen de perfil de un usuario determinado.
        /// </summary>
        /// <param name="CodeUser"></param>
        /// <returns></returns>
        public Task<ServiceResponse<bool>> DeleteImageProfileUserAsync(string CodeUser);
    }
}

