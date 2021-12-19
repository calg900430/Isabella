namespace Isabella.Web.Helpers.RepositoryHelpers
{
    
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    using Isabella.Web.Models;

    /// <summary>
    /// Repositorio Génerico
    /// </summary>
    public interface IServiceGenericRepository<TEntity> where TEntity : class
    {
        /// <summary>
        /// Implementa un metodo generico que devuelve una entidad con las relaciones indicadas.
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public Task<TEntity> GetLoadAsync(Expression<Func<TEntity, bool>> Id,
        params Expression<Func<TEntity, object>>[] parameters);
    }
}
