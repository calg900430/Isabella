namespace Isabella.Web.Controllers.API
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Common.Dtos.CalificationRestaurant;
    using Repositorys.API;

    /// <summary>
    ///Controlador CalificationRestaurants.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CalificationRestaurantController : Controller
    {
        private readonly ICalificationRestaurantRepositoryAPI _calificationRestaurantService;

        /// <summary>
        /// Constructor
        /// </summary>
        public CalificationRestaurantController(ICalificationRestaurantRepositoryAPI calificationRestaurantService)
        {
            this._calificationRestaurantService = calificationRestaurantService;
        }

        //POST
        /// <summary>
        /// Agrega un nueva calificación de un usuario acerca del restaurante.
        /// </summary>
        /// <param name="addCalificationRestaurant"></param>
        /// <returns></returns>
        [HttpPost()]
        [ProducesResponseType(200, Type = typeof(bool))]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> PostAddClasificationProductAsync([FromBody] AddCalificationRestaurantDto addCalificationRestaurant)
        {
            if (ModelState.IsValid)
            {
                var execute_get = await this._calificationRestaurantService.AddCalificationForRestaurantAsync(addCalificationRestaurant);
                if (execute_get.Data == true)
                return Ok(execute_get); //200
                else
                return NotFound(execute_get); //404
            }
            else
            return BadRequest(); //400
        }
    }
}
