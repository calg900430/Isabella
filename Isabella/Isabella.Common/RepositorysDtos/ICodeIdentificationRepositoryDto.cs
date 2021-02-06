namespace Isabella.Common.RepositorysDtos
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Repositorio para el manejo de los Dtos de los códigos de identificación.
    /// </summary>
    public interface ICodeIdentificationRepositoryDto
    {
        /// <summary>
        /// Solicita el código un de identificación para registrarse e iniciar sesión en la aplicación
        /// </summary>
        /// <returns></returns>
        Task<ServiceResponse<Guid?>> GetCodeIdentificationAsync();

        /// <summary>
        /// Verifica si el código de identificación está registrado.
        /// </summary>
        /// <param name="CodeIdentification"></param>
        /// <returns></returns>
        Task<ServiceResponse<bool>> CheckCodeIdentificationAsync(Guid CodeIdentification);
    }
}
