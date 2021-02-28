namespace Isabella.Common.RepositorysDtos
{
    using System.Threading.Tasks;
    using System.Collections.Generic;
   
    using Common;
    using Dtos.Users;

    /// <summary>
    /// Repositorio para el manejo de los Dtos de los usuarios en general.
    /// </summary>
    public partial interface IUserRepositoryDto
    {
        /// <summary>
        /// Establece si un usuario se banea o no.
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="banner"></param>
        /// <returns></returns>
        Task<ServiceResponse<bool>> BannerUserAsync(int UserId, bool banner);

        /// <summary>
        /// Obtiene todos los usuarios disponibles con datos disponibles solo para usuarios admin.
        /// </summary>
        /// <returns></returns>
        Task<ServiceResponse<List<GetUserAllDataForOnlyAdmin>>> GetAllDataOfUserAsync();

        /// <summary>
        /// Obtiene un usuario dado su Id con datos disponibles solo para usuarios admin
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        Task<ServiceResponse<GetUserAllDataForOnlyAdmin>> GetAllDataOfOnlyUserAsync(int UserId);

        /// <summary>
        /// Obtiene los roles disponibles de un usuario.
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        Task<ServiceResponse<List<string>>> GetAllRoleOfUser(int UserId);

        /// <summary>
        /// Obtiene todos los usuarios del sistema.
        /// </summary>
        /// <returns></returns>
        Task<ServiceResponse<List<GetUserDto>>> GetAllUserAsync();

        /// <summary>
        /// Obtiene todos los usuarios clientes.
        /// </summary>
        /// <returns></returns>
        Task<ServiceResponse<List<GetUserDto>>> GetAllUserClientAsync();

        /// <summary>
        /// Elimina el role owner de un usuario.
        /// </summary>
        /// <returns></returns>
        Task<ServiceResponse<bool>> RemoveRoleOwnerAsync(int UserId);

        /// <summary>
        /// Elimina el role admin de un usuario.
        /// </summary>
        /// <returns></returns>
        Task<ServiceResponse<bool>> RemoveRoleAdminAsync(int UserId);

        /// <summary>
        /// Agrega el role owner a un usuario.
        /// </summary>
        /// <returns></returns>
        Task<ServiceResponse<bool>> AssigningRoleOwnerToUserAsync(int UserId);

        /// <summary>
        /// Agrega el role admin a un usuario.
        /// </summary>
        /// <returns></returns>
        Task<ServiceResponse<bool>> AssigningRoleAdminToUserAsync(int UserId);

        /// <summary>
        /// Agrega el role client a un usuario.
        /// </summary>
        /// <returns></returns>
        Task<ServiceResponse<bool>> AssigningRoleClientToUserAsync(int UserId);

        /// <summary>
        /// Agrega un usuario admin en el sistema.
        /// </summary>
        /// <param name="newuser"></param>
        /// <returns></returns>
        Task<ServiceResponse<bool>> AddUserAdminAsync(RegisterUserDto newuser);

        /// <summary>
        /// Agrega un usuario owner en el sistema.
        /// </summary>
        /// <param name="newuser"></param>
        /// <returns></returns>
        Task<ServiceResponse<bool>> AddUserOwnerAsync(RegisterUserDto newuser);

        /// <summary>
        /// Agrega un usuario cliente en el sistema.
        /// </summary>
        /// <param name="newuser"></param>
        /// <returns></returns>
        Task<ServiceResponse<bool>> AddUserClientAsync(RegisterUserDto newuser);

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
        Task<ServiceResponse<GetDataUserForLoginDto>> LoginUserAsync(LoginUserWithUserNameDto loginUser);

        /// <summary>
        /// Confirma el registro del usuario.
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="Token"></param>
        /// <returns></returns>
        Task<ServiceResponse<bool>> ConfirmEmailUserAsync(string Id, string Token);

        /// <summary>
        /// Solicita un correo con un código para recuperaión de su contraseña.
        /// </summary>
        /// <param name="resetPassword"></param>
        /// <returns></returns>
        Task<ServiceResponse<bool>> ResetPasswordUserAsync(ResetPasswordDto resetPassword);

        /// <summary>
        /// Recupera la contraseña del usuario con los detalles enviados a su correo. 
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="Token"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        Task<ServiceResponse<bool>> RecoverPasswwordUserAsync(string Id, string Token, string newPassword);

        /// <summary>
        /// Envia un nuevo correo  con detalles para la confirmación del registro.
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

