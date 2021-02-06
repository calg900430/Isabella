namespace Isabella.Common.RepositorysDtos
{
    using System.Threading.Tasks;
    using System.Collections.Generic;
   
    using Common;
    using Dtos.Users;
    using Extras;

    /// <summary>
    /// Repositorio para el manejo de los Dtos de los usuarios(En este caso solo para los usuarios admin en el dashboard).
    /// </summary>
    public interface IUserRepositoryDto
    {

        /// <summary>
        /// Obtiene todos los usuarios del sistema.
        /// </summary>
        /// <returns></returns>
        Task<ServiceResponse<List<GetUserDto>>> GetAllUserAsync();

        /// <summary>
        /// Agrega un usuario en el sistema.
        /// </summary>
        /// <param name="newuser"></param>
        /// <returns></returns>
        Task<ServiceResponse<bool>> AddUserAsync(RegisterUserDto newuser);

        /// <summary>
        /// Obtiene el Id del último usuario en la base de datos.
        /// </summary>
        /// <returns></returns>
        Task<ServiceResponse<int>> GetIdOfLastUserAsync();

        /// <summary>
        /// Obtiene un usuario dada su cuenta de usuario.
        /// </summary>
        /// <param name="UserName"></param>
        /// <returns></returns>
        Task<ServiceResponse<GetUserDto>> GetUserByUserNameAsync(string UserName);

        /// <summary>
        /// Obtiene un usuario dado su Id.
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        Task<ServiceResponse<GetUserDto>> GetUserByIdAsync(int UserId);

        /// <summary>
        /// Obtiene un usuario dado su correo electrónico.
        /// </summary>
        /// <param name="Email"></param>
        /// <returns></returns>
        Task<ServiceResponse<GetUserDto>> GetUserByEmailAsync(string Email);

        /// <summary>
        /// Devuelve una lista de usuarios a partir de uno de referencia.
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="CantUsers"></param>
        /// <returns></returns>
        Task<ServiceResponse<List<GetUserDto>>> GetCantUserAsync(int UserId, int CantUsers);

        /// <summary>
        /// Cambia la contraseña del usuario.
        /// </summary>
        /// <param name="changePassword"></param>
        /// <returns></returns>
        Task<ServiceResponse<bool>> UpdatePasswordAsync(ChangePasswordUserDto changePassword);

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
        Task<ServiceResponse<ResponseLoginTokenDto>> LoginUserAsync(LoginUserWithUserNameDto loginUser);

        /// <summary>
        /// Confirma el registro del usuario.
        /// </summary>
        /// <param name="confirmEmail"></param>
        /// <returns></returns>
        Task<ServiceResponse<bool>> ConfirmEmailUserAsync(ConfirmEmailDto confirmEmail);

        /// <summary>
        /// Solicita un correo con un código para recuperaión de su contraseña.
        /// </summary>
        /// <param name="resetPassword"></param>
        /// <returns></returns>
        Task<ServiceResponse<bool>> ResetPasswordUserAsync(ResetPasswordDto resetPassword);

        /// <summary>
        /// Recupera la contraseña del usuario con el código enviado a su correo. 
        /// </summary>
        /// <param name="recoverPassword"></param>
        /// <returns></returns>
        Task<ServiceResponse<bool>> RecoverPasswwordUserAsync(RecoverPasswordDto recoverPassword);

        /// <summary>
        /// Envia un nuevo código para la confirmación del registro.
        /// </summary>
        /// <param name="sendToNewCodeConfirmationRegister"></param>
        /// <returns></returns>
        Task<ServiceResponse<bool>> SendToNewCodeConfirmationEmailAsync(SendToNewCodeConfirmationRegisterDto sendToNewCodeConfirmationRegister);

        /// <summary>
        /// Borra la imagen de perfil de un usuario determinado.
        /// </summary>
        /// <returns></returns>
        Task<ServiceResponse<bool>> DeleteImageProfileUserAsync();

        // <summary>
        /// Login de usuario con proveedor externo(Google, Facebook, Apple y Twitter), si el usuario no existe lo registra.
        /// </summary>
        /// <param name="scheme"></param>
        /// <returns></returns>
        Task LoginExternProviderForAsync(string scheme);
    }
}

