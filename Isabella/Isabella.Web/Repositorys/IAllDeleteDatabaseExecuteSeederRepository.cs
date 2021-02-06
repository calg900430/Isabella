namespace Duma.API.Repositorys
{
    using Duma.Common;
    using System.Threading.Tasks;

    /// <summary>
    /// Repositorio de pruebas para eliminar el contenido de las tablas de la base de datos.
    /// </summary>
    public interface IAllDeleteDatabaseExecuteSeederRepository
    {
        /// <summary>
        /// Borra todas la base de datos y ejecuta el Seeder
        /// </summary>
        public Task<ServiceResponse<bool>> DeleteAllAsync();
    }
}
