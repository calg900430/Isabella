namespace Isabella.Web.Controllers
{
    using Isabella.Web.ServicesControllers;
    using Isabella.Web.ViewModels.UsersViewModel;
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
    }
}
