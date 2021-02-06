namespace Isabella.Web.Controllers.API_RESTful
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;

    using Repositorys.API; 
    using Common.Dtos.Users;
    using Common;

    /// <summary>
    /// Controlador para el servicio de usuarios.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        private readonly IUserRepositoryAPI _userService;
       
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="userService"></param>
        public UsersController(IUserRepositoryAPI userService)
        {
            this._userService = userService;
        }

        /// <summary>
        /// Obtiene todos los usuarios del sistema.
        /// </summary>
        /// <returns></returns>
        [HttpGet("get/alluser")]
        [ProducesResponseType(200, Type = typeof(List<GetUserDto>))]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
        public async Task<IActionResult> GetAllUserAsync()
        {
            if (ModelState.IsValid)
            {
                var response = await this._userService.GetAllUserAsync().ConfigureAwait(false);
                if (response.Success)
                return Ok(response);
                else
                return BadRequest(response); //400
            }
            else
            return BadRequest(); //400
        }

        /// <summary>
        /// Obtiene un usuario dado su cuenta de usuario.
        /// </summary>
        /// <param name="UserName"></param>
        /// <returns></returns>
        [HttpGet("get/username")]
        [ProducesResponseType(200, Type = typeof(GetUserDto))]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetUserForUserNameAsync(string UserName)
        {
            if (ModelState.IsValid)
            {
                var response = await this._userService.GetUserByUserNameAsync(UserName).ConfigureAwait(false);
                if (response.Success)
                return Ok(response);
                else
                return BadRequest(response); //400
            }
            else
            return BadRequest(); //400
        }

        /// <summary>
        /// Obtiene un usuario dado su Id.
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        [HttpGet("get/id")]
        [ProducesResponseType(200, Type = typeof(GetUserDto))]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetUserForUserIdAsync(int UserId)
        {
            if (ModelState.IsValid)
            {
                var response = await _userService.GetUserByIdAsync(UserId).ConfigureAwait(false);
                if (response.Success)
                return Ok(response);
                else
                return BadRequest(response); //400
            }
            else
            return BadRequest(); //400
        }

        /// <summary>
        /// Obtiene un usuario dado su email.
        /// </summary>
        /// <param name="Email"></param>
        /// <returns></returns>
        [HttpGet("get/email")]
        [ProducesResponseType(200, Type = typeof(GetUserDto))]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetUserForUserEmailAsync(string Email)
        {
            if (ModelState.IsValid)
            {
                var response = await this._userService.GetUserByEmailAsync(Email).ConfigureAwait(false);
                if(response.Success)
                return Ok(response);
                else
                return BadRequest(response); //400
            }
            else
            return BadRequest(); //400
        }

        //POST
        /// <summary>
        /// Registra un usuario de owner(dueño) y le envia un correo con el código de confirmación.
        /// </summary>
        /// <param name="newuser"></param>
        /// <returns></returns>
        [HttpPost("register/userclient")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
        public async Task<IActionResult> PostRegisterUserTatooAsync([FromBody] RegisterUserDto newuser)
        {
            if (ModelState.IsValid)
            {
                var response = await this._userService.AddUserAsync(newuser, "owner").ConfigureAwait(false);
                if (response.Success)
                return Ok(response);
                else
                return BadRequest(response); //400
            }
            else
            return BadRequest(); //400
        }

        //POST
        /// <summary>
        /// Registra un usuario de rol client y le envia un correo con el código de confirmación.
        /// </summary>
        /// <param name="newuser"></param>
        /// <returns></returns>
        [HttpPost("register/user")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> PostRegisterUserClientAsync([FromBody] RegisterUserDto newuser)
        {
            if (ModelState.IsValid)
            {
                var response = await this._userService.AddUserAsync(newuser, "client").ConfigureAwait(false);
                if (response.Success)
                return Ok(response);
                else
                return BadRequest(response); //400
            }
            else
            return BadRequest(); //400
        }

        //POST
        /// <summary>
        /// Confirma el registro de un usuario en el sistema.
        /// </summary>
        /// <param name="confirmEmail"></param>
        /// <returns></returns>
        [HttpPost("confirmregister")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> PostConfirmRegisterUserAsync([FromBody] ConfirmEmailDto confirmEmail)
        {
            if (ModelState.IsValid)
            {
               var response = await this._userService.ConfirmEmailUserAsync(confirmEmail).ConfigureAwait(false);
               if (response.Success)
               return Ok(response);
               else
               return BadRequest(response); //400
            }
            else
            return BadRequest(); //400
        }

        //POST
        /// <summary>
        /// Recupera la contraseña del usuario con el código de recuperación.
        /// </summary>
        /// <param name="recoverPassword"></param>
        /// <returns></returns>
        [HttpPost("recoverpassword")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> PostConfirmRecoverPasswordUserAsync([FromBody] RecoverPasswordDto recoverPassword)
        {
            if (ModelState.IsValid)
            {
                var response = await this._userService.RecoverPasswwordUserAsync(recoverPassword).ConfigureAwait(false);
                if (response.Success)
                return Ok(response);
                else
                return BadRequest(response); //400
            }
            else
            return BadRequest(); //400
        }

        //POST
        /// <summary>
        /// Envia un correo al usuario con el código de recuperación de la contraseña.
        /// </summary>
        /// <param name="resetPassword"></param>
        /// <returns></returns>
        [HttpPost("getcode_recoverpassword")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> PostGetEmailForRecoverPasswordUserAsync([FromBody] ResetPasswordDto resetPassword)
        {
            if (ModelState.IsValid)
            {
                var response = await this._userService.ResetPasswordUserAsync(resetPassword).ConfigureAwait(false);
                if (response.Success)
                return Ok(response);
                else
                return BadRequest(response); //400
            }
            else
             return BadRequest(); //400
        }

        //POST
        /// <summary>
        /// Loguea a un usuario y obtiene los del usuario, el Token de acceso,el tiempo de expiración y el código de usuario.
        /// </summary>
        /// <param name="loginuser"></param>
        /// <returns></returns>
        [HttpPost("login")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> PostApiLoginUserAsync([FromBody] LoginUserWithUserNameDto loginuser)
        {
            if (ModelState.IsValid)
            {
                var response = await this._userService.LoginUserForApiAsync(loginuser).ConfigureAwait(false);
                if (response.Success)
                return Ok(response);
                else
                return BadRequest(response); //400
            } 
            else
            return BadRequest(); //400
        }

        //POST
        /// <summary>
        /// Cambia la contraseña de un usuario.
        /// </summary>
        /// <param name="changePassword"></param>
        /// <returns></returns>
        [HttpPost("update/password")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> PostChangePasswordUserAsync([FromBody] ChangePasswordUserDto changePassword)
        {
            if (ModelState.IsValid)
            {
                var response = await this._userService.UpdatePasswordAsync(changePassword).ConfigureAwait(false);
                if (response.Success)
                return Ok(response);
                else
                return BadRequest(response); //400
            }
            else
            return BadRequest(); //400
        }

        //PUT
        /// <summary>
        /// Actualiza un usuario.
        /// </summary>
        /// <param name="updateuser"></param>
        /// <returns></returns>
        [HttpPut("update/user")]
        [ProducesResponseType(200, Type =typeof(GetUserDto))]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> PutUpdateUserAsync([FromBody] UpdateUserDto updateuser)
        {
            if (ModelState.IsValid)
            {
                var response = await this._userService.UpdateUserAsync(updateuser).ConfigureAwait(false);
                if (response.Success)
                return Ok(response);
                else
                return BadRequest(response); //400
            }
            else
            return BadRequest(); //400
        }

        /// <summary>
        /// Banea un usuario del sistema.
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        [HttpPut("banner")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
        public async Task<IActionResult> BannerUserAsync(int UserId)
        {
            if (ModelState.IsValid)
            {
                var response = await this._userService.BannerUserAsync(UserId).ConfigureAwait(false);
                if (response.Success)
                return Ok(response);
                else
                return BadRequest(response); //400
            }
            else
            return BadRequest(); //400 
        }
    }
}
