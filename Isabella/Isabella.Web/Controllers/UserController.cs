namespace Isabella.Web.Controllers
{
    using Isabella.Common.Dtos.Users;
    using Isabella.Web.ServicesControllers;
    using Isabella.Web.ViewModels.UsersViewModel;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// User
    /// </summary>
    [Route("[controller]")]
    [ApiExplorerSettings(IgnoreApi = true)] //Omite este controlador de la documentación API
    public class UserController : Controller
    {
        private readonly UserServiceController _userServiceController;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="userServiceController"></param>
        public UserController(UserServiceController userServiceController)
        {
            this._userServiceController = userServiceController;
        }

        /// <summary>
        /// Obtiene todos los usuarios administradores.
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Index()
        {
            try
            {
                var list_users_admins = await this._userServiceController
                .GetAllUserAdminsAsync().ConfigureAwait(false);
                if (list_users_admins.Success)
                {
                    var getAllUsersAdmins = list_users_admins.Data
                    .Select(c => new GetUserAdminViewModel
                    {
                        Id = c.Id,
                        FirstName = c.FirstName,
                        LastName = c.LastName,
                        Email = c.Email,
                        ImageUserProfile = c.ImageUserProfile
                    }).ToList();
                    return View(getAllUsersAdmins);
                }
                else
                {
                    //TODO:Retorna página de que no existen los productos.
                    return BadRequest(list_users_admins);
                }
            }
            catch (Exception ex)
            {
                //TODO:Retorna página de excepciones con el mensaje del motivo de la Excepcion.
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Muestra la View para el Login.
        /// </summary>
        /// <returns></returns>
        [HttpGet("login")]
        public IActionResult Login()
        {
            if (this.User.Identity.IsAuthenticated && this.User.IsInRole("admin"))
            {
                return this.RedirectToAction("Dashboard", "Home");
            }
            return this.View();
        }

        /// <summary>
        /// Realiza la autenticación web.
        /// </summary>
        /// <param name="loginViewModel"></param>
        /// <returns></returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            try
            {
                if (this.ModelState.IsValid)
                {
                    var result = await this._userServiceController
                    .LoginUserWebOnlyAdminAsync(loginViewModel)
                    .ConfigureAwait(false);
                    if (result.Success)
                    {
                        if (this.Request.Query.Keys.Contains("ReturnUrl"))
                        {
                            return this.Redirect(this.Request.Query["ReturnUrl"].First());
                        }
                        return this.RedirectToAction("Dashboard", "Home");
                    }
                    else
                    {
                       //Retornar Vista de que no está autorizado
                       return this.RedirectToAction("_NotAuthorized", "Home");
                    }
                }
                else
                {
                    return BadRequest();
                }                
            }
            catch
            {
               //Mostrar pagina de control de excepciones.
               return this.RedirectToAction("_ServerError", "Home");
            }
            
        }

        /// <summary>
        /// Cierra la sesión web actual.
        /// </summary>
        /// <returns></returns>
        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            await this._userServiceController.LogoutUserWebAsync();
            return this.RedirectToAction("Index", "Home");  
        }


        /// <summary>
        /// Muestra la vista para crear un nuevo usuario admin.
        /// </summary>
        /// <returns></returns>
        [HttpGet("create")]
        [Authorize(Roles = "admin")]
        public IActionResult Create()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                //Muestra la pagina de control de excepciones
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Agrega un nuevo usuario admin y le envia un correo con el código de confirmación del registro.
        /// </summary>
        /// <param name="newuser"></param>
        /// <returns></returns>
        [HttpPost("create")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create([FromForm] AddUserViewModel newuser)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //Obtiene la dirección Url
                    this._userServiceController.Url = Url;
                    this._userServiceController.HttpRequest = HttpContext.Request;
                    var result = await this._userServiceController
                    .AddUserAdminAsync(newuser).ConfigureAwait(false);
                    if (result.Success)
                    return Ok(result); 
                    else
                    return NotFound(result);
                }
                else
                return BadRequest(); //400
            }
            catch (Exception ex)
            {
                //Muestra la pagina de control de excepciones
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Confirma el registro de un usuario en la aplicación con los detalles enviados a su correo electrónico.
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="Token"></param>
        /// <returns></returns>
        [HttpPost("confirmregister")]
        public async Task<IActionResult> ConfirmRegister(string Id, string Token)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._userServiceController
                    .ConfirmEmailUserAsync(Id, Token)
                    .ConfigureAwait(false);
                    if (result.Success)
                      return View();
                    else
                      return NotFound(result);
                }
                else
                    return BadRequest(); //400
            }
            catch (Exception ex)
            {
                //Mostrar pagina de control de excepciones
                return View(ex.Message);
            }
        }

        /// <summary>
        /// Envia un correo al usuario con los detalles para la recuperación de la contraseña.
        /// </summary>
        /// <param name="resetPassword"></param>
        /// <returns></returns>
        [HttpPost("resetpassword")]
        public async Task<IActionResult> ResetPassword([FromForm] ResetPasswordDto resetPassword)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    this._userServiceController.Url = Url;
                    this._userServiceController.HttpRequest = HttpContext.Request;
                    var result = await this._userServiceController.ResetPasswordUserAsync(resetPassword).ConfigureAwait(false);
                    if (result.Success)
                     return Ok(result);
                    else
                     return NotFound(result);
                }
                else
                    return BadRequest(); //400
            }
            catch (Exception ex)
            {
                //Mostrar pagina de control de excepciones
                return View(ex.Message);
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
        public async Task<IActionResult> ConfirmRecoverPasswordUserAsync(string Id, string Token, string NewPassword)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //Asigna los claims del usuario
                    var result = await this._userServiceController
                    .RecoverPasswwordUserAsync(Id, Token, NewPassword)
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
        /// Envia un nuevo correo al usuario con los detalles para la confirmación del registro.
        /// </summary>
        /// <param name="sendToNewCodeConfirmationRegister"></param>
        /// <returns></returns>
        [HttpPost("add/new_email_confirmregister")]
        public async Task<IActionResult> NewEmailConfirmRegisterUserAsync([FromBody] SendToNewCodeConfirmationRegisterDto sendToNewCodeConfirmationRegister)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    this._userServiceController.Url = Url;
                    this._userServiceController.HttpRequest = HttpContext.Request;
                    var result = await this._userServiceController
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
        /// Borra un usuario del sistema.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("delete")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = true;
                    /*var result = await this._userServiceController
                    .DeleteUserAsync().ConfigureAwait(false);*/
                    if (result)
                     return Ok(result);
                    else
                     return NotFound(result);
                }
                else
                    return BadRequest(); //400
            }
            catch (Exception ex)
            {
                //Muestra la pagina de control de excepciones
                return BadRequest(ex.Message);
            }
        }
    }
}
