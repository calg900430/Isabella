namespace Isabella.Web.Controllers.API
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;

    using Common;
    using Common.Dtos.Categorie;
    using Common.Dtos.Aggregate;
    using Common.RepositorysDtos;
    using Models;
    using ServicesControllers;
   

    /// <summary>
    /// Controlador para los productos standard.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AggregateController : Controller
    {
        private readonly AggregateServiceController _aggregateServiceController;

        /// <summary>
        /// Servicio de los agregados.
        /// </summary>
        /// <param name="aggregateServiceController"></param>
        public AggregateController(AggregateServiceController aggregateServiceController)
        {
            this._aggregateServiceController = aggregateServiceController;
        }

        /// <summary>
        /// Agregar un nuevo agregado.
        /// </summary>
        /// <param name="aggregate"></param>
        /// <returns></returns>
        [HttpPost("add/product")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
        public async Task<IActionResult> AddAggregateAsync([FromBody] AddAggregateDto aggregate)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._aggregateServiceController
                    .AddAggregateAsync(aggregate).ConfigureAwait(false);
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
        /// Actualiza un agregado.
        /// </summary>
        /// <param name="updateAggregate"></param>
        /// <returns></returns>
        [HttpPut("update/product")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
        public async Task<IActionResult> UpdateAggregateAsync([FromBody] UpdateAggregateDto updateAggregate)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._aggregateServiceController
                    .UpdateAggregateAsync(updateAggregate)
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
        /// Obtiene un agregado dado su Id.
        /// </summary>
        /// <param name="AggregateId"></param>
        /// <returns></returns>
        [HttpGet("get/id")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetAggregateForIdAsync(int AggregateId)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._aggregateServiceController
                    .GetAggregateForIdAsync(AggregateId)
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
        /// Obtiene todos los agregados.
        /// </summary>
        /// <returns></returns>
        [HttpGet("get/all")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetAllAggregateAsync()
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._aggregateServiceController
                    .GetAllAggregateAsync()
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
        /// Obtiene el Id del último agregado que se registró en el sistema.
        /// </summary>
        /// <returns></returns>
        [HttpGet("get/id_last_product")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetIdOfLastAggregateAsync()
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._aggregateServiceController
                    .GetIdOfLastAggregateAsync()
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
        /// Accede una imagen especifica de un agregado.
        /// </summary>
        /// <returns></returns>
        [HttpGet("getimage/product")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetImageAggregateAsync(int AggregateId, int ImageId)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._aggregateServiceController
                    .GetImageAggregateForIdAsync(AggregateId, ImageId)
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
        /// Agrega una imagen de un agregado(Usando IFormFile).
        /// </summary>
        /// <param name="formFile"></param>
        /// <param name="AggregateId"></param>
        /// <returns></returns>
        [HttpPost("addimagesformfile/product")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
        public async Task<IActionResult> AddImageAggregateAsync(IFormFile formFile, int AggregateId)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._aggregateServiceController
                    .AddImageAggregateAsync(formFile, AggregateId)
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
        /// Agrega una imagen a un agregado(Usando Dto).
        /// </summary>
        /// <param name="addImageAggregate"></param>
        /// <returns></returns>
        [HttpPost("addimagesdto/product")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
        public async Task<IActionResult> AddImageAggregateAsync([FromBody] AddImageAggregateDto addImageAggregate)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._aggregateServiceController
                    .AddImageAggregateAsync(addImageAggregate)
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
        /// Borra una imagen de un agregado.
        /// </summary>
        /// <returns></returns>
        [HttpDelete("deleteimage/product")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
        public async Task<IActionResult> DeleteImageAggregateAsync(int AggregateId, int ImageId)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._aggregateServiceController
                    .DeleteImageAggregateAsync(AggregateId, ImageId)
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
        /// Establece un agregado en disponible o no disponible.
        /// </summary>
        /// <param name="AggregateId"></param>
        /// <param name="enable"></param>
        /// <returns></returns>
        [HttpPost("enable_product")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
        public async Task<IActionResult> EnableProductAggregateAsync(int AggregateId, bool enable)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._aggregateServiceController
                    .EnableAggregateAsync(AggregateId, enable)
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
        /// Obtiene una cantidad determinada de productos dado un agregado de referencia y la cantidad.
        /// </summary>
        /// <param name="AggregateId"></param>
        /// <param name="cantAggregate"></param>
        /// <returns></returns>
        [HttpGet("get/cant")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetCantAggregateAsync(int AggregateId, int cantAggregate)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._aggregateServiceController
                    .GetCantAggregateAsync(AggregateId, cantAggregate)
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
        /// Obtiene todos los Id de las imagenes de un agregado.
        /// </summary>
        /// <param name="AggregateId"></param>
        /// <returns></returns>
        [HttpGet("get/all_listid_images")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetListIdOfImageAggregateAsync(int AggregateId)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._aggregateServiceController
                    .GetListIdOfImageAggregateAsync(AggregateId)
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
        /// Obtiene todas las imagenes de un agregado.
        /// </summary>
        /// <param name="AggregatedId"></param>
        /// <returns></returns>
        [HttpGet("getallimage/product")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetAllImageAggregatedAsync(int AggregatedId)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._aggregateServiceController
                    .GetAllImageAggregateAsync(AggregatedId)
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
        /// Obtiene una cantidad especifica de imagenes de un agregado.
        /// </summary>
        /// <param name="AggregateId"></param>
        /// <param name="ImageId"></param>
        /// <param name="cantImages"></param>
        /// <returns></returns>
        [HttpGet("getcantimage/product")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetCantImageAggregateAsync(int AggregateId, int ImageId, int cantImages)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._aggregateServiceController
                    .GetCantImageAggregateAsync(AggregateId, ImageId, cantImages)
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
        /// Obtiene un agregado dado su Id si el mismo está disponible.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet("get/id_available")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetAggregateIsAvailableForIdAsync(int Id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._aggregateServiceController
                    .GetAggregateIsAvailableForIdAsync(Id)
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
        /// Obtiene una cantidad determinada de agregados disponibles dado un agregado de referencia y la cantidad.
        /// </summary>
        /// <param name="AggregateId"></param>
        /// <param name="CantAggregate"></param>
        /// <returns></returns>
        [HttpGet("get/cant_available")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetCantAggregateIsAvailabelAsync(int AggregateId, int CantAggregate)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._aggregateServiceController
                    .GetCantAggregateIsAvailableAsync(AggregateId, CantAggregate)
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
        /// Obtiene todos los agregados disponibles.
        /// </summary>
        /// <returns></returns>
        [HttpGet("get/all_available")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetAllAggregateIsAvailableAsync()
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._aggregateServiceController
                    .GetAllAggregateIsAvailableAsync()
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
        /// Elimina un agregado.
        /// </summary>
        /// <param name="AggregateId"></param>
        /// <returns></returns>
        [HttpDelete("delete/aggregate")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(401)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
        public async Task<IActionResult> DeleteAggregateAsync(int AggregateId)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._aggregateServiceController
                    .DeleteAggregateAsync(AggregateId)
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
