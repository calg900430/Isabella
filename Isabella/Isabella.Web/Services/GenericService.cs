namespace Duma.API.Services
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;

    using Repositorys;
    using Data;
    using Extras;

    /// <summary>
    /// Servicio Génerico.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GenericService<T> : IGenericRepository<T> where T : class, IModel
    {
        private DataContext _dataContext;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataContext"></param>
        public GenericService(DataContext dataContext)
        {
           this._dataContext = dataContext;
        }

        /// <summary>
        /// Guarda los cambios en la base de datos.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> SaveAllAsync()
        {
            return await _dataContext.SaveChangesAsync().ConfigureAwait(false) > 0;
        }

        /// <summary>
        ///  Accede a un elemento por su Id.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<T> GetByIdAsync(int Id)
        {
           return await _dataContext.Set<T>().AsNoTracking().FirstOrDefaultAsync(e => e.Id == Id).ConfigureAwait(false);
        }


        /// <summary>
        /// Guarda una entidad en la base de datos y guarda los cambios en la base de datos.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<T> CreateAsync(T entity)
        {
            await _dataContext.Set<T>().AddAsync(entity).ConfigureAwait(false);
            await SaveAllAsync().ConfigureAwait(false);
            return entity;
        }

        /// <summary>
        /// Borra un elemento de la base de datos y guarda los cambios en la base de datos.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task DeleteAsync(T entity)
        {
            this._dataContext.Set<T>().Remove(entity);
            await SaveAllAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Verifica si un elemento existe en la base de datos.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<bool> ExistAsync(int Id)
        {
            var entity = await _dataContext.Set<T>().FirstOrDefaultAsync(e => e.Id == Id).ConfigureAwait(false);
            if(entity == null)
            return false;
            else
            return true;
        }

        /// <summary>
        /// Actualiza un elemento en la base de datos y guarda los cambios en la base de datos.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<T> UpdateAsync(T entity)
        {
            this._dataContext.Set<T>().Update(entity);
            await SaveAllAsync().ConfigureAwait(false);
            return entity;
        }
    }
}
