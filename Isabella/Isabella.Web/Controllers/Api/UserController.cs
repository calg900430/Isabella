namespace Isabella.Web.Controllers.API
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Authentication;

    using Common.Dtos.Users;
    using ServicesControllers;
   
    /// <summary>
    /// Controlador para el servicio de usuarios.
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class UserController : Controller
    {
        private readonly UserServiceController _userRepository;
        
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="userRepositoryDto"></param>
        public UserController(UserServiceController userRepositoryDto)
        {
            this._userRepository = userRepositoryDto;
        }

        /// <summary>
        /// Registro rapido de usuario(Solicita el código de identificación para registrarse e iniciar sesión en la aplicación)
        /// Le crea un correo y un password al usuario, que se le devuelve para que el mismo haga el login.
        /// En este modo no tiene que confirmar el registro a través del correo.
        /// </summary>
        /// <returns></returns>
        [HttpGet("add_mode_fast/new_user")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> AddUserModeFastAsync()
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._userRepository.RegisterUserModeFastAsync().ConfigureAwait(false);
                    if (result.Success)
                    return Ok(result);
                    else
                    return BadRequest(result);
                }
                else
                    return BadRequest(); //400
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Agrega un nuevo usuario y le envia un correo con el código de confirmación del registro.
        /// </summary>
        /// <param name="newuser"></param>
        /// <returns></returns>
        [HttpPost("add/new_user")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> AddUserAsync([FromBody] RegisterUserDto newuser)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //Obtiene la dirección Url
                    this._userRepository.Url = Url;
                    this._userRepository.HttpRequest = HttpContext.Request;
                    var result = await this._userRepository.AddUserAsync(newuser).ConfigureAwait(false);
                    if(result.Success)
                    return Ok(result);
                    else
                    return BadRequest(result);
                }
                else
                return BadRequest(); //400
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            } 
        }

        /// <summary>
        /// Confirma el registro de un usuario en la aplicación con los detalles enviados a su correo electrónico.
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="Token"></param>
        /// <returns></returns>
        [HttpPost("add/confirmregister")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> ConfirmRegisterUserAsync(string Id, string Token)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._userRepository.ConfirmEmailUserAsync(Id, Token).ConfigureAwait(false);
                    if (result.Success)
                    return Ok(result);
                    else
                    return BadRequest(result);
                }
                else
                return BadRequest(); //400
            }
            catch (Exception ex)
            {
               return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Borra la imagen de perfil del usuario.
        /// </summary>
        /// <returns></returns>
        [HttpDelete("delete/imageuserprofile")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> DeleteImageProfileUserAsync()
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //Asigna los claims del usuario
                    this._userRepository.ClaimsPrincipal = HttpContext.User;
                    var result = await this._userRepository.DeleteImageProfileUserAsync().ConfigureAwait(false);
                    if (result.Success)
                    return Ok(result);
                    else
                    return BadRequest(result);
                }
                else
                return BadRequest(); //400
            }
            catch (Exception ex)
            {
               return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Obtiene el Id del último usuario que se registró en el sistema.
        /// </summary>
        /// <returns></returns>
        [HttpGet("get/id_lastuser")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetIdOfLastUserAsync()
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._userRepository.GetIdOfLastUserAsync().ConfigureAwait(false);
                    if (result.Success)
                    return Ok(result);
                    else
                    return BadRequest(result);
                }
                else
                return BadRequest(); //400
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Obtiene un usuario dado su Id.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("get/id")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetUserForUserIdAsync(int userId)
        {
            try
            {   
                if (ModelState.IsValid)
                {
                    var result = await this._userRepository.GetUserByIdAsync(userId).ConfigureAwait(false);
                    if (result.Success)
                    return Ok(result);
                    else
                    return BadRequest(result);
                }
                else
                return BadRequest(); //400
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Obtiene todos los usuarios del sistema.
        /// </summary>
        /// <returns></returns>
        [HttpGet("get/allusers")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetAllUserAsync()
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._userRepository.GetAllUserAsync().ConfigureAwait(false);
                    if (result.Success)
                    return Ok(result);
                    else
                    return BadRequest(result);
                }
                else
                return BadRequest(); //400
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Obtiene todos los usuarios del sistema con un determinado rol.
        /// </summary>
        /// <returns></returns>
        [HttpGet("get/allusers_for_role")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
        public async Task<IActionResult> GetAllUserAsync(int RoleId)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._userRepository.GetAllUserWithRoleAsync(RoleId).ConfigureAwait(false);
                    if (result.Success)
                    return Ok(result);
                    else
                    return BadRequest(result);
                }
                else
                return BadRequest(); //400
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Obtiene un usuario dada su cuenta de usuario.
        /// </summary>
        /// <param name="UserName"></param>
        /// <returns></returns>
        [HttpGet("get/username")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetUserForUserNameAsync(string UserName)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._userRepository.GetUserByUserNameAsync(UserName).ConfigureAwait(false);
                    if (result.Success)
                    return Ok(result);
                    else
                    return BadRequest(result);
                }
                else
                return BadRequest(); //400
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Obtiene un usuario dado su email.
        /// </summary>
        /// <param name="Email"></param>
        /// <returns></returns>
        [HttpGet("get/email")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetUserForUserEmailAsync(string Email)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._userRepository.GetUserByEmailAsync(Email).ConfigureAwait(false);
                    if (result.Success)
                    return Ok(result);
                    else
                    return BadRequest(result);
                }
                else
                return BadRequest(); //400
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Login del usuario y obtiene el token de acceso,el tiempo de expiración y otros datos del usuario.
        /// </summary>
        /// <param name="loginUser"></param>
        /// <returns></returns>
        [HttpPost("login")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> LoginUserAsync([FromBody] LoginUserDto loginUser)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._userRepository.LoginUserAsync(loginUser).ConfigureAwait(false);
                    if (result.Success)
                    return Ok(result);
                    else
                    return BadRequest(result);
                }
                else
                return BadRequest(); //400
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Login de usuarios con proveedores externos(Google, Facebook, Twitter y Apple). 
        /// Si el usuario no existe lo registra en la aplicación.
        /// </summary>
        /// <param name="scheme"></param>
        /// <returns></returns>
        [HttpGet("login/extern_provider")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task LoginExternProviderForAsync(string scheme)
        {
            try
            {
                if (ModelState.IsValid)
                {
                   //Asigna los claims del usuario
                   this._userRepository.ClaimsPrincipal = HttpContext.User;
                   this._userRepository.HttpRequest = Request.HttpContext.Request;
                   await this._userRepository.LoginExternProviderForAsync(scheme).ConfigureAwait(false);
                }
                else
                return;
            }
            catch(Exception)
            {
              return;
            }
        }

        /// <summary>
        /// Recupera la contraseña del usuario con los detalles enviados a su correo electrónico.
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="Token"></param>
        /// <param name="NewPassword"></param>
        /// <returns></returns>
        [HttpPost("add/recoverpassword")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> ConfirmRecoverPasswordUserAsync(string Id, string Token, string NewPassword)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //Asigna los claims del usuario
                    var result = await this._userRepository.RecoverPasswwordUserAsync(Id, Token, NewPassword).ConfigureAwait(false);
                    if (result.Success)
                    return Ok(result);
                    else
                    return BadRequest(result);
                }
                else
                return BadRequest(); //400
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Obtiene un número determinado de usuarios dado un Id y la cantidad deseada.
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="CantUsers"></param>
        /// <returns></returns>
        [HttpGet("get/cant_users")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetUsersForIdAndCantAsync(int UserId, int CantUsers)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._userRepository.GetCantUserAsync(UserId,CantUsers).ConfigureAwait(false);
                    if (result.Success)
                    return Ok(result);
                    else
                    return BadRequest(result);
                }
                else
                return BadRequest(); //400
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Actualiza un usuario.
        /// </summary>
        /// <param name="updateUser"></param>
        /// <returns></returns>
        [HttpPut("update/user")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> UpdateUserAsync([FromBody] UpdateUserDto updateUser)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    this._userRepository.ClaimsPrincipal = HttpContext.User;
                    var result = await this._userRepository.UpdateUserAsync(updateUser).ConfigureAwait(false);
                    if (result.Success)
                    return Ok(result);
                    else
                    return BadRequest(result);
                }
                else
                return BadRequest(); //400
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Cambia la contraseña de un usuario.
        /// </summary>
        /// <param name="changePassword"></param>
        /// <returns></returns>
        [HttpPut("update/password")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> ChangePasswordUserAsync([FromBody] ChangePasswordUserDto changePassword)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    this._userRepository.ClaimsPrincipal = HttpContext.User;
                    var result = await this._userRepository.UpdatePasswordAsync(changePassword).ConfigureAwait(false);
                    if (result.Success)
                    return Ok(result);
                    else
                    return BadRequest(result);
                }
                else
                return BadRequest(); //400
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Envia un correo al usuario con los detalles para la recuperación de la contraseña.
        /// </summary>
        /// <param name="resetPassword"></param>
        /// <returns></returns>
        [HttpPost("add/code_recoverpassword")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> EmailForRecoverPasswordUserAsync([FromBody] ResetPasswordDto resetPassword)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    this._userRepository.Url = Url;
                    this._userRepository.HttpRequest = HttpContext.Request;
                    var result = await this._userRepository.ResetPasswordUserAsync(resetPassword).ConfigureAwait(false);
                    if (result.Success)
                    return Ok(result);
                    else
                    return BadRequest(result);
                }
                else
                return BadRequest(); //400
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Envia un nuevo correo al usuario con los detalles para la confirmación del registro.
        /// </summary>
        /// <param name="sendToNewCodeConfirmationRegister"></param>
        /// <returns></returns>
        [HttpPost("add/new_email_confirmregister")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> NewEmailConfirmRegisterUserAsync([FromBody] SendToNewCodeConfirmationRegisterDto sendToNewCodeConfirmationRegister)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    this._userRepository.Url = Url;
                    this._userRepository.HttpRequest = HttpContext.Request;
                    var result = await this._userRepository
                    .SendToNewCodeConfirmationEmailAsync(sendToNewCodeConfirmationRegister)
                    .ConfigureAwait(false);
                    if (result.Success)
                    return Ok(result);
                    else
                    return BadRequest(result);
                }
                else
                return BadRequest(); //400
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Agrega o actualiza una imagen de perfil de un usuario determinado usando IFormFile.
        /// </summary>
        /// <param name="formFile"></param>
        /// <returns></returns>
        [HttpPost("add/imageuserprofile")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> AddImageProfileUserAsync(IFormFile formFile)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    this._userRepository.Url = Url;
                    var result = await this._userRepository
                    .AddOrUpdateImageProfileUser(formFile, HttpContext.User)
                    .ConfigureAwait(false);
                    if (result.Success)
                    return Ok(result);
                    else
                    return BadRequest(result);
                }
                else
                return BadRequest(); //400
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        /// <summary>
        /// Elimina un role determinado de un usuario dado.
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="RoleId"></param>
        /// <returns></returns>
        [HttpDelete("delete/role_admin")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
        public async Task<IActionResult> RemoveRoleOfUserAsync(int UserId, int RoleId)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._userRepository
                    .RemoveRoleInUserAsync(UserId, RoleId)
                    .ConfigureAwait(false);
                    if (result.Success)
                    return Ok(result);
                    else
                    return BadRequest(result);
                }
                else
                return BadRequest(); //400
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        /// <summary>
        /// Obtiene todos los usuarios con todos sus datos, disponible solo para usuarios admins.
        /// </summary>
        /// <returns></returns>
        [HttpGet("get/alldata_allusers")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
        public async Task<IActionResult> GetAllUserAllDataAsync()
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._userRepository
                    .GetAllDataOfUserAsync()
                    .ConfigureAwait(false);
                    if (result.Success)
                    return Ok(result);
                    else
                    return BadRequest(result);
                }
                else
                    return BadRequest(); //400
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Obtiene todos los datos de un usuario, disponible solo para usuarios admins.
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        [HttpGet("get/alldata_user")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
        public async Task<IActionResult> GetUserAllDataAsync(int UserId)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._userRepository
                    .GetAllDataOfOnlyUserAsync(UserId)
                    .ConfigureAwait(false);
                    if (result.Success)
                        return Ok(result);
                    else
                        return BadRequest(result);
                }
                else
                    return BadRequest(); //400
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Asigna un role determinado a un usuario.
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="RoleId"></param>
        /// <returns></returns>
        [HttpPost("add/role")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
        public async Task<IActionResult> AddRoleToUserAsync(int UserId, int RoleId)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._userRepository
                    .AssigningRoleToUserAsync(UserId, RoleId)
                    .ConfigureAwait(false);
                    if (result.Success)
                    return Ok(result);
                    else
                    return BadRequest(result);
                }
                else
                return BadRequest(); //400
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Indica si un usuario es baneado o no.
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="banner"></param>
        /// <returns></returns>
        [HttpPost("banner_user")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
        public async Task<IActionResult> UserBannerUserAsync(int UserId, bool banner)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._userRepository
                    .BannerUserAsync(UserId, banner)
                    .ConfigureAwait(false);
                    if (result.Success)
                    return Ok(result);
                    else
                    return BadRequest(result);
                }
                else
                return BadRequest(); //400
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Elimina el usuario admin de la lista de los que reciben las notificaciones de las ordenes.
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        [HttpDelete("delete/user_for_recive_notifications")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
        public async Task<IActionResult> RemoveUserAdminForNotifications(int UserId)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._userRepository
                    .RemoveUserAdminForNotifications(UserId)
                    .ConfigureAwait(false);
                    if (result.Success)
                    return Ok(result);
                    else
                    return BadRequest(result);
                }
                else
                return BadRequest(); //400
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Agrega un usuario admin a la lista de los que reciben las notificaciones de las ordenes.
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        [HttpPost("add/user_for_recive_notifications")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
        public async Task<IActionResult> AddUserAdminForNotifications(int UserId)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._userRepository
                    .AddUserAdminForNotifications(UserId)
                    .ConfigureAwait(false);
                    if (result.Success)
                    return Ok(result);
                    else
                    return BadRequest(result);
                }
                else
                    return BadRequest(); //400
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Actualiza o agrega el chat id de un usuario admin para recibir por Telegram las notificaciones de las ordenes.
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="ChatIdTelegram"></param>
        /// <returns></returns>
        [HttpPost("add_update/chatid")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
        public async Task<IActionResult> AddOrUpdateChatIdTelegramUserAdminForNotifications(int UserId, int ChatIdTelegram)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._userRepository
                    .UpdateChatIdTelegramUserAdminForNotifications(UserId, ChatIdTelegram)
                    .ConfigureAwait(false);
                    if (result.Success)
                    return Ok(result);
                    else
                    return BadRequest(result);
                }
                else
                return BadRequest(); //400
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
