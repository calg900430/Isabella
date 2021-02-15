namespace Isabella.API.ServicesControllers
{
    using System;
    using System.Threading.Tasks;

    using Common.RepositorysDtos;
    using Common;
    using Common.Extras;
    using Helpers;
    using Helpers.RepositoryHelpers;
    using Models.Entities;
 
    /// <summary>
    /// Servicio para el controlador de los códigos de identificación.
    /// </summary>
    public class CodeIdentificationServiceController : ICodeIdentificationRepositoryDto
    {
        private readonly ServiceGenericHelper<CodeIdentification> _serviceGenericCodeIdentificationHelper;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serviceGenericCodeIdentificationHelper"></param>
        public CodeIdentificationServiceController(ServiceGenericHelper<CodeIdentification>  serviceGenericCodeIdentificationHelper)
        {
            this._serviceGenericCodeIdentificationHelper = serviceGenericCodeIdentificationHelper;
        }

        /// <summary>
        /// Verifica si el código de identificación está registrado.
        /// </summary>
        /// <param name="CodeVerification"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<bool>> CheckCodeIdentificationAsync(Guid CodeVerification)
        {
            ServiceResponse<bool> serviceResponse = new ServiceResponse<bool>();
            try
            {
                //Verifica si el código de identificación está disponible
                var codeidentification = await this._serviceGenericCodeIdentificationHelper
                .WhereFirstEntityAsync(c => c.Code == CodeVerification)
                .ConfigureAwait(false);
                if(codeidentification == null)
                {
                    serviceResponse.KeyResource = GetValueResourceFile.KeyResource.NotCodeIdentification;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = GetValueResourceFile
                    .GetValueResourceString(GetValueResourceFile.KeyResource.NotCodeIdentification);
                    return serviceResponse;
                }
                serviceResponse.KeyResource = GetValueResourceFile.KeyResource.SuccessOk;
                serviceResponse.Data = true;
                serviceResponse.Success = true;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.SuccessOk);
                return serviceResponse;
            }
            catch
            {
                serviceResponse.KeyResource = GetValueResourceFile.KeyResource.Exception;
                serviceResponse.Data = false;
                serviceResponse.Success = false;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.Exception);
                return serviceResponse;
            }
        }

        /// <summary>
        /// Solicita el código de identificación para registrarse e iniciar sesión en la aplicación
        /// </summary>
        /// <returns></returns>
        public async Task<ServiceResponse<Guid?>> GetCodeIdentificationAsync()
        {
            ServiceResponse<Guid?> serviceResponse = new ServiceResponse<Guid?>();
            try
            {
                var code_identification = new CodeIdentification
                {
                    Code = Guid.NewGuid()
                };
                 await this._serviceGenericCodeIdentificationHelper
                .AddEntityAsync(code_identification)
                .ConfigureAwait(false);
                await this._serviceGenericCodeIdentificationHelper
                .SaveChangesBDAsync()
                .ConfigureAwait(false);
                serviceResponse.KeyResource = GetValueResourceFile.KeyResource.SuccessOk;
                serviceResponse.Data = code_identification.Code;
                serviceResponse.Success = true;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.SuccessOk);
                return serviceResponse;
            }
            catch
            {
                serviceResponse.KeyResource = GetValueResourceFile.KeyResource.Exception;
                serviceResponse.Data =  null;
                serviceResponse.Success = false;
                serviceResponse.Message = GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.Exception);
                return serviceResponse;
            }
        }
    }
}
