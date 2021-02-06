namespace Isabella.API.ServicesModels
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;

    using Data;
    using RepositorysModels;
    using Models;

    /// <summary>
    /// Servicio para la entidad que representa los códigos de identificación.
    /// </summary>
    public class CodeIdentificationServiceModel : ICodeIdentificationModel
    {
        private readonly DataContext _dataContext;

        /// <summary>
        /// 
        /// </summary>
        public CodeIdentificationServiceModel(DataContext dataContext)
        {
            this._dataContext = dataContext;
        }

        /// <summary>
        /// Verifica si un código de identificación está registrado.
        /// </summary>
        /// <returns></returns>
        public async Task<CodeIdentification> CheckCodeIdentificationAsync(Guid Code)
        => await this._dataContext.CodeIdentifications
        .FirstOrDefaultAsync(c => c.Code == Code)
        .ConfigureAwait(false);

        /// <summary>
        /// Registra un nuevo código de identificación.
        /// </summary>
        /// <returns></returns>
        public async Task<CodeIdentification> RegisterNewCodeIdentification()
        {
            var newcode = new CodeIdentification
            {
                Code = Guid.NewGuid()
            };
            await this._dataContext.CodeIdentifications.AddAsync(newcode).ConfigureAwait(false);
            await this._dataContext.SaveChangesAsync();
            return newcode;
        }
    }
}
