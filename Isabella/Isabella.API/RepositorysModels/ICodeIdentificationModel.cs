namespace Isabella.API.RepositorysModels
{
   
    using System;
    using System.Threading.Tasks;

    using Models;

    /// <summary>
    /// Repositorio para los códigos de identificación.
    /// </summary>
    public interface ICodeIdentificationModel
    {
        /// <summary>
        /// Verifica si el código de identificación está registrado.
        /// </summary>
        /// <param name="Code"></param>
        /// <returns></returns>
        public Task<CodeIdentification> CheckCodeIdentificationAsync(Guid Code);

        /// <summary>
        /// Solicita el código de identificación para registrarse e iniciar sesión en la aplicación.        /// </summary>
        /// <returns></returns>
        public Task<CodeIdentification> RegisterNewCodeIdentification();
    }
}
