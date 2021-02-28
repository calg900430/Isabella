namespace Isabella.API.Controllers
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
    using Common.Dtos.Category;
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
        private readonly AggregateServiceController _aggregateRepository;
      
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="aggregateRepository"></param>
        public AggregateController(AggregateServiceController aggregateRepository)
        {
            this._aggregateRepository = aggregateRepository;
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
                    var result = await this._aggregateRepository.AddAggregateAsync(aggregate).ConfigureAwait(false);
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
        public async Task<IActionResult> UpdateAggregateAsync(UpdateAggregateDto updateAggregate)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._aggregateRepository
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
        //[ProducesResponseType(401)]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetAggregateForIdAsync(int AggregateId)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._aggregateRepository
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
        //[ProducesResponseType(401)]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetAllAggregateAsync()
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._aggregateRepository
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
        //[ProducesResponseType(401)]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetIdOfLastAggregateAsync()
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._aggregateRepository
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
        //[ProducesResponseType(401)]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetImageAggregateAsync(int AggregateId, int ImageId)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._aggregateRepository
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
                    var result = await this._aggregateRepository
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
        public async Task<IActionResult> AddImageAggregateAsync(AddImageAggregateDto addImageAggregate)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._aggregateRepository
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
                    var result = await this._aggregateRepository
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
                    var result = await this._aggregateRepository
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
        //[ProducesResponseType(401)]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetCantAggregateAsync(int AggregateId, int cantAggregate)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._aggregateRepository
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
        //[ProducesResponseType(401)]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetListIdOfImageAggregateAsync(int AggregateId)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._aggregateRepository
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
        //[ProducesResponseType(401)]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetAllImageAggregatedAsync(int AggregatedId)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._aggregateRepository
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
        //[ProducesResponseType(401)]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetCantImageAggregateAsync(int AggregateId, int ImageId, int cantImages)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._aggregateRepository
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
        //[ProducesResponseType(401)]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetAggregateIsAvailableForIdAsync(int Id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._aggregateRepository
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
        //[ProducesResponseType(401)]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetCantAggregateIsAvailabelAsync(int AggregateId, int CantAggregate)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._aggregateRepository
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
        //[ProducesResponseType(401)]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetAllAggregateIsAvailableAsync()
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._aggregateRepository
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
        [HttpDelete("delete/product")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        //[ProducesResponseType(403)]
        //[ProducesResponseType(401)]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> DeleteProductAsync(int AggregateId)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._aggregateRepository
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
