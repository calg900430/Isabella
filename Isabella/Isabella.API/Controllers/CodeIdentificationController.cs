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
    using Common.Extras;
    using Common.RepositorysDtos;
    using ServicesControllers;

    /// <summary>
    /// Controlador para los códigos de verificación.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class CodeIdentificationController : Controller
    {
        private readonly CodeIdentificationServiceController _codeIdentificationRepository;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="codeVerificationRepository"></param>
        public CodeIdentificationController(CodeIdentificationServiceController codeVerificationRepository)
        {
            this._codeIdentificationRepository = codeVerificationRepository;
        }

        /// <summary>
        /// Verifica si el código de identificación es correcto.
        /// </summary>
        /// <returns></returns>
        [HttpGet("get/check_codeverification")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CheckCodeVerificationAsync(Guid Code)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._codeIdentificationRepository
                    .CheckCodeIdentificationAsync(Code)
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
        /// Solicita un código de identificación para registrarse e iniciar sesión en la aplicación.
        /// </summary>
        /// <returns></returns>
        [HttpGet("get/request_codeverification")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GeCodeVerificationAsync()
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this._codeIdentificationRepository
                    .GetCodeIdentificationAsync()
                    .ConfigureAwait(false);
                    if (result.Success)
                    return Ok(result);
                    else
                    return BadRequest(result);
                }
                else
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
