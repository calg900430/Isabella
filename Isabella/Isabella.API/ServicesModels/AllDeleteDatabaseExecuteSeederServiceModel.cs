namespace Isabella.API.ServicesModels
{
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;

    using Data;
    using RepositorysModels;
    using SeederDb ;
    using Common;
    using Common.Extras;

    /// <summary>
    /// Servicio de pruebas para eliminar el contenido de las tablas de la base de datos.
    /// </summary>
    public class AllDeleteDatabaseExecuteSeederServiceModel
    {
        private readonly DataContext _dataContext;
        private readonly SeedDb _seederDb;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataContext"></param>
        /// <param name="seederDb"></param>
        public AllDeleteDatabaseExecuteSeederServiceModel(DataContext dataContext, SeedDb seederDb)
        {
            this._dataContext = dataContext;
            this._seederDb = seederDb;
        }

        /// <summary>
        /// Borra todas la base de datos y ejecuta el Seeder
        /// </summary>
        public async Task<ServiceResponse<bool>> DeleteAllAsync()
        {
            ServiceResponse<bool> serviceResponse = new ServiceResponse<bool>();
            try
            {
                //Gps
                if (await this._dataContext.Gps.AnyAsync().ConfigureAwait(false))
                {
                    //Obtiene las coordenadas gps disponibles
                    var all_gps = await this._dataContext.Gps
                    .ToListAsync();
                    //Borra toda las coordenadas gps disponibles
                    this._dataContext.Gps.RemoveRange(all_gps);
                }
                //UserRoles
                if (await this._dataContext.UserRoles.AnyAsync().ConfigureAwait(false))
                {
                    //Obtiene las relaciones de roles disponibles
                    var all_roles_relation = await this._dataContext.UserRoles
                    .ToListAsync();
                    //Borra toda las relaciones de roles disponibles
                    this._dataContext.UserRoles.RemoveRange(all_roles_relation);
                }
                //Roles
                if (await this._dataContext.Roles.AnyAsync().ConfigureAwait(false))
                {
                    //Obtiene los de roles disponibles
                    var all_roles = await this._dataContext.Roles
                    .ToListAsync();
                    //Borra toda las relaciones de roles disponibles
                    this._dataContext.Roles.RemoveRange(all_roles);
                }
                //Users
                if (await this._dataContext.Users.AnyAsync().ConfigureAwait(false))
                {
                    //Obtiene los de roles disponibles
                    var all_roles = await this._dataContext.Users
                    .ToListAsync();
                    //Borra toda las relaciones de roles disponibles
                    this._dataContext.Users.RemoveRange(all_roles);
                }
                //ConfirmationRegisterForEmail
                if (await this._dataContext.ConfirmationRegisterForEmail.AnyAsync().ConfigureAwait(false))
                {
                    //Obtiene las relaciones de las contraseñas de confirmación
                    var all_confirmation_email = await this._dataContext.ConfirmationRegisterForEmail
                    .ToListAsync();
                    //Borra toda las relaciones de las contraseñas de confirmación
                    this._dataContext.ConfirmationRegisterForEmail.RemoveRange(all_confirmation_email);
                }
                //RecoverPassword
                if (await this._dataContext.RecoverPasswords.AnyAsync().ConfigureAwait(false))
                {
                    //Obtiene las relaciones de las contraseñas de recuperación
                    var all_recover_passwords = await this._dataContext.RecoverPasswords
                    .ToListAsync();
                    //Borra toda las relaciones de las contraseñas de recuperación
                    this._dataContext.RecoverPasswords.RemoveRange(all_recover_passwords);
                }
                await this._dataContext.SaveChangesAsync();
                //Ejecuta el Seeder
                await this._seederDb.SeedAsync().ConfigureAwait(false);
                serviceResponse.Data = true;
                serviceResponse.Code = CodeMessage.Code.CodeSuccess_Ok;
                serviceResponse.Success = true;
                serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeSuccess_Ok);
                return serviceResponse;
            }
           catch
            {
               serviceResponse.Data = false;
               serviceResponse.Code = CodeMessage.Code.CodeError_Exception;
               serviceResponse.Success = false;
               serviceResponse.Message = CodeMessage.MessageOfCode(CodeMessage.Code.CodeError_Exception);
               return serviceResponse;
            }
        }
    }
}
