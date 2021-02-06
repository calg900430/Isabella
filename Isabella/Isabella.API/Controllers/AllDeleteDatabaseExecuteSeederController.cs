namespace Isabella.API.Controllers
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Net;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;

    using RepositorysModels;
    using Common;
    using Common.Extras;
    using ServicesModels;

    /// <summary>
    /// Controlador para el servicio de login con proveedores externos.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
    public class AllDeleteDatabaseExecuteSeederController : Controller
    {
        private readonly AllDeleteDatabaseExecuteSeederServiceModel _allDeleteDatabaseExecuteSeeder;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="allDeleteDatabaseExecuteSeeder"></param>
        public AllDeleteDatabaseExecuteSeederController(AllDeleteDatabaseExecuteSeederServiceModel allDeleteDatabaseExecuteSeeder)
        {
            this._allDeleteDatabaseExecuteSeeder = allDeleteDatabaseExecuteSeeder;
        }

        /// <summary>
        /// Borra todos los elementos disponibles en la base de datos y ejecuta el Seeder(Solo para pruebas en modo de desarrollo).
        /// </summary>
        /// <returns></returns>
        [HttpDelete("delete_all")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> DeleteAllExecuteSeederAsync()
        {
            if (ModelState.IsValid)
            {
                var response = await this._allDeleteDatabaseExecuteSeeder.DeleteAllAsync().ConfigureAwait(false);
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
