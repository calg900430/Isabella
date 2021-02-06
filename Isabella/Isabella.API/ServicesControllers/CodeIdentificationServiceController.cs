namespace Isabella.API.ServicesControllers
{
    using System;
    using System.Threading.Tasks;

    using Common.RepositorysDtos;
    using RepositorysModels;
    using Common;
    using Common.Extras;

    /// <summary>
    /// Servicio para el controlador de los códigos de identificación.
    /// </summary>
    public class CodeIdentificationServiceController : ICodeIdentificationRepositoryDto
    {
        private readonly ICodeIdentificationModel _verificationCodeModel;
    
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="verificationCodeModel"></param>
        public CodeIdentificationServiceController(ICodeIdentificationModel verificationCodeModel)
        {
            this._verificationCodeModel = verificationCodeModel;
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
                var codeidentification = await this._verificationCodeModel
                .CheckCodeIdentificationAsync(CodeVerification)
                .ConfigureAwait(false);
                if (codeidentification == null)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeIdentification_NotCode;
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeIdentification_NotCode);
                    return serviceResponse;
                }
                serviceResponse.Code = CodeMessage.Code.CodeSuccess_Ok;
                serviceResponse.Data = true;
                serviceResponse.Success = true;
                serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeSuccess_Ok);
                return serviceResponse;
            }
            catch
            {
                serviceResponse.Code = CodeMessage.Code.CodeError_Exception;
                serviceResponse.Data = false;
                serviceResponse.Success = false;
                serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeError_Exception);
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
                //Verifica si el código de identificación está disponible
                var codeidentification = await this._verificationCodeModel.RegisterNewCodeIdentification()
                .ConfigureAwait(false);
                if (codeidentification == null)
                {
                    serviceResponse.Code = CodeMessage.Code.CodeError_DataBase;
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeError_DataBase);
                    return serviceResponse;
                }
                serviceResponse.Code = CodeMessage.Code.CodeSuccess_Ok;
                serviceResponse.Data = codeidentification.Code;
                serviceResponse.Success = true;
                serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeSuccess_Ok);
                return serviceResponse;
            }
            catch
            {
                serviceResponse.Code = CodeMessage.Code.CodeError_Exception;
                serviceResponse.Data = null;
                serviceResponse.Success = false;
                serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeError_Exception);
                return serviceResponse;
            }
        }
    }
}
