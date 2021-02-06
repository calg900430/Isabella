namespace Isabella.API.Controllers
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

    using RepositorysModels;
    using Common.Dtos.Users;
    using Common.Extras;
    using Common.RepositorysDtos;
    using Common;
    using Extras;
    using Models;
    using ServicesControllers;
  
    /// <summary>
    /// Controlador para el servicio de usuarios.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        private readonly UserServiceController _userRepositoryDto;
        private readonly IUserRepositoryModel _userRepositoryModel;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="userRepositoryDto"></param>
        /// <param name="userRepositoryModel"></param>
        public UsersController(UserServiceController userRepositoryDto, IUserRepositoryModel userRepositoryModel)
        {
            this._userRepositoryDto = userRepositoryDto;
            this._userRepositoryModel = userRepositoryModel;
        }

        /// <summary>
        /// Agrega un usuario y le envia un correo con el código de confirmación del registro(Agrega un usuario admin solo, puede hacerlo otro usuario admin).
        /// </summary>
        /// <param name="newuser"></param>
        /// <returns></returns>
        [HttpPost("add/new_user")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
        public async Task<IActionResult> AddUserAsync([FromBody] RegisterUserDto newuser)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._userRepositoryDto.AddUserAsync(newuser).ConfigureAwait(false);
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
        /// Confirma el registro de un usuario en el sistema con el código enviado a su correo.
        /// </summary>
        /// <param name="confirmEmail"></param>
        /// <returns></returns>
        [HttpPost("add/confirmregister")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> ConfirmRegisterUserAsync([FromBody] ConfirmEmailDto confirmEmail)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._userRepositoryDto.ConfirmEmailUserAsync(confirmEmail).ConfigureAwait(false);
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
        [ProducesResponseType(403)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
        public async Task<IActionResult> DeleteImageProfileUserAsync()
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //Asigna los claims del usuario
                    this._userRepositoryDto.ClaimsPrincipal = HttpContext.User;
                    var result = await this._userRepositoryDto.DeleteImageProfileUserAsync().ConfigureAwait(false);
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
        [ProducesResponseType(403)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
        public async Task<IActionResult> GetIdOfLastUserAsync()
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._userRepositoryDto.GetIdOfLastUserAsync().ConfigureAwait(false);
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
        [ProducesResponseType(403)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
        public async Task<IActionResult> GetUserForUserIdAsync(int userId)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._userRepositoryDto.GetUserByIdAsync(userId).ConfigureAwait(false);
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
        [ProducesResponseType(403)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
        public async Task<IActionResult> GetAllUserAsync()
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._userRepositoryDto.GetAllUserAsync().ConfigureAwait(false);
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
        [ProducesResponseType(403)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
        public async Task<IActionResult> GetUserForUserNameAsync(string UserName)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._userRepositoryDto.GetUserByUserNameAsync(UserName).ConfigureAwait(false);
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
        [ProducesResponseType(403)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
        public async Task<IActionResult> GetUserForUserEmailAsync(string Email)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._userRepositoryDto.GetUserByEmailAsync(Email).ConfigureAwait(false);
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
        public async Task<IActionResult> LoginUserAsync([FromBody] LoginUserWithUserNameDto loginUser)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._userRepositoryDto.LoginUserAsync(loginUser).ConfigureAwait(false);
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
            const string callbackScheme = "xamarinessentials";
            try
            {
                if (scheme != "Google")
                return;
                //Verifica si los datos enviados por el proveedor externo son correctos.
                var auth = await Request.HttpContext.AuthenticateAsync(scheme);
                if (!auth.Succeeded || auth?.Principal == null
                || !auth.Principal.Identities.Any(id => id.IsAuthenticated)
                || string.IsNullOrEmpty(auth.Properties.GetTokenValue("access_token")))
                {
                    //Not authenticated, challenge
                    await Request.HttpContext.ChallengeAsync(scheme);
                    return;
                }
                else
                {
                    var claims = auth.Principal.Identities.FirstOrDefault()?.Claims;
                    var email = string.Empty;
                    //var mobilePhone = string.Empty;
                    email = claims?.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Email)?.Value;
                    //mobilePhone = claims?.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.MobilePhone)?.Value;
                    //TODO: Resolver el login externo en caso de que el usuario tenga telefono en vez de email(Facebook o Twitter)
                    //Verifica que el usuario este registrado, si no está registrado lo registra.
                    var user = await this._userRepositoryDto.GetEntityUserForEmailAsync(email).ConfigureAwait(false);
                    //Registra el nuevo usuario
                    if (user == null)
                    {
                        user = new User()
                        {
                            Email = email,
                            UserName = email,
                            DateCreated = DateTime.UtcNow,
                            DateUpdated = DateTime.UtcNow,
                            LastDateConnected = DateTime.UtcNow,
                            IdForClaim = Guid.NewGuid(),
                        };
                        var register_user = await this._userRepositoryModel.AddUserAsync(user).ConfigureAwait(false);
                        if (register_user == null)
                        {
                            await Request.HttpContext.ChallengeAsync(scheme);
                            return;
                        }
                        //Asigna el role al usuario
                        var role_user = await this._userRepositoryModel.AddRoleForUserAsync(user, Constants.RolesOfSystem[1]).ConfigureAwait(false);
                        if (!role_user)
                        {
                            await Request.HttpContext.ChallengeAsync(scheme);
                            return;
                        }
                    }
                    //Genera un Token Bearer para el usuario
                    var token = await this._userRepositoryModel.CreateTokenAsync(user).ConfigureAwait(false);
                    if (token == null)
                    {
                        await Request.HttpContext.ChallengeAsync(scheme);
                        return;
                    }
                    //Verifica si el usuario tiene foto de perfil
                    var image = string.Empty;
                    if (user.ImageUserProfile != null)
                    image = Convert.ToBase64String(user.ImageUserProfile);
                    //Verifica los roles del usuario
                    var role = string.Empty;
                    if (token.UserRoles != null)
                    {
                        foreach (EnumRoles enumRoles in token.UserRoles)
                        role += $"{(int)enumRoles},";
                    }
                    //Crea los parametros que devolveremos a la url back
                    var qs = new Dictionary<string, string>
                    {
                      { "access_token", auth.Properties.GetTokenValue("access_token") },
                      { "refresh_token", auth.Properties.GetTokenValue("refresh_token") ?? string.Empty },
                      { "expires", (auth.Properties.ExpiresUtc?.ToUnixTimeSeconds() ?? -1).ToString() },
                      { "token_bearer", token.Token},
                      { "expires_token_bearer", token.DateTime.ToString()},
                      { "email", user.Email},
                      { "roles", role},
                      { "imageUserProfile", image ?? string.Empty},
                      { "userName", user.UserName ?? string.Empty},
                      { "firstName", user.FirstName ?? string.Empty },
                      { "lastName", user.LastName ?? string.Empty},
                      { "fullName", user.FullName ?? string.Empty},
                      { "id", user.Id.ToString() ?? string.Empty},
                      { "address", user.Address ?? string.Empty},
                      { "phoneNumber", user.PhoneNumber ?? string.Empty},
                    };
                    // Build the result url
                    var url = callbackScheme + "://#" + string.Join("&",
                    qs.Select(kvp => $"{kvp.Key}={kvp.Value}"));
                    // Redirect to final url
                    Request.HttpContext.Response.Redirect(url);
                    return;
                }
            }
            catch (Exception)
            {
                await Request.HttpContext.ChallengeAsync(scheme);
                return;
            }
        }

        /// <summary>
        /// Recupera la contraseña del usuario con el código de recuperación enviado a su correo.
        /// </summary>
        /// <param name="recoverPassword"></param>
        /// <returns></returns>
        [HttpPost("add/recoverpassword")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> ConfirmRecoverPasswordUserAsync([FromBody] RecoverPasswordDto recoverPassword)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //Asigna los claims del usuario
                    this._userRepositoryDto.ClaimsPrincipal = HttpContext.User;
                    var result = await this._userRepositoryDto.RecoverPasswwordUserAsync(recoverPassword).ConfigureAwait(false);
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
        [ProducesResponseType(403)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
        public async Task<IActionResult> GetUsersForIdAndCantAsync(int UserId, int CantUsers)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._userRepositoryDto.GetCantUserAsync(UserId,CantUsers).ConfigureAwait(false);
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
        [ProducesResponseType(403)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
        public async Task<IActionResult> UpdateUserAsync([FromBody] UpdateUserDto updateUser)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    this._userRepositoryDto.ClaimsPrincipal = HttpContext.User;
                    var result = await this._userRepositoryDto.UpdateUserAsync(updateUser).ConfigureAwait(false);
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
        [ProducesResponseType(403)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
        public async Task<IActionResult> ChangePasswordUserAsync([FromBody] ChangePasswordUserDto changePassword)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    this._userRepositoryDto.ClaimsPrincipal = HttpContext.User;
                    var result = await this._userRepositoryDto.UpdatePasswordAsync(changePassword).ConfigureAwait(false);
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
        /// Envia un correo al usuario con el código de recuperación de la contraseña.
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
                    var result = await this._userRepositoryDto.ResetPasswordUserAsync(resetPassword).ConfigureAwait(false);
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
        /// Envia un nuevo correo al usuario con el código para la confirmación del registro.
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
                    var result = await this._userRepositoryDto
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
        [ProducesResponseType(403)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
        public async Task<IActionResult> AddImageProfileUserAsync(IFormFile formFile)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._userRepositoryDto
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
    }
}
