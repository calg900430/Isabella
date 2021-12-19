namespace Isabella.Web.Helpers
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Resources;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using Microsoft.EntityFrameworkCore;

    using Data;
    using Models;
    using Isabella.Web.Resources;
    using Isabella.Web.Helpers.RepositoryHelpers;

    /// <summary>
    /// Helper que representa un servicio generico para realizar CRUD sobre las entidades.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class ServiceGenericHelper<TEntity> : IServiceGenericRepository<TEntity> where TEntity : class, IEntity
    {
        /// <summary>
        /// Contexto.
        /// </summary>
        private readonly DataContext _dataContext;

        /// <summary>
        /// Representa la tabla de la entidad.
        /// </summary>
        private DbSet<TEntity> m_dbSet;

        /// <summary>
        /// Devuelve el contexto actual de datos.
        /// </summary>
        public DbSet<TEntity> _context { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="dataContext"></param>
        public ServiceGenericHelper(DataContext dataContext)
        {
            this._dataContext = dataContext;
            m_dbSet = this._dataContext.Set<TEntity>();
            _context = m_dbSet;
        }

        #region Métodos Públicos
        /// <summary>
        /// Devuelve una entidad con las relaciones indicada.
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual async Task<TEntity> GetLoadAsync(Expression<Func<TEntity, bool>> Id,
        params Expression<Func<TEntity, object>>[] parameters)
        {
            IQueryable<TEntity> query = m_dbSet.AsQueryable();
            foreach (Expression<Func<TEntity, object>> parameter in parameters)
            {
                query = query.Include(parameter);
            }
            return await query.FirstOrDefaultAsync(Id).ConfigureAwait(false);
        }

        /// <summary>
        /// Obtiene todas las entidades disponibles con las entidades relacionadas deseadas.
        /// </summary>
        /// <returns></returns>
        public virtual async Task<List<TEntity>> GetLoadAsync(params Expression<Func<TEntity, object>>[] parameters)
        {
            IQueryable<TEntity> query = m_dbSet.AsQueryable();
            if (await query.AnyAsync().ConfigureAwait(false))
            {
                foreach (Expression<Func<TEntity, object>> entitie in parameters)
                {
                    query = query.Include(entitie);
                }
                return await query.ToListAsync<TEntity>().ConfigureAwait(false);
            }
            return null;
        }

        /// <summary>
        /// Obtiene una cantidad determinada de entidades a partir de una entidad dada más la cantidad deseada.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="CantEntity"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual async Task<List<TEntity>> GetLoadAsync(TEntity entity, int CantEntity,
        params Expression<Func<TEntity, object>>[] parameters)
        {
            if (entity == null)
                throw new NullEntityException(GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.EntityIsNull));
            if (CantEntity < 0)
                throw new CantNegativeException(GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.CantIsNegative));
            //Crea la consulta para determinar las entidades relacionadas deseadas.
            IQueryable<TEntity> query = m_dbSet.AsQueryable();
            if (await query.AnyAsync().ConfigureAwait(false))
            {
                foreach (Expression<Func<TEntity, object>> entitie in parameters)
                {
                    query = query.Include(entitie);
                }
            }
            //Agrega a la consulta la cantidad de entidades que desea y partir de cual debe tomarlas.
            return await query.Where(c => c.Id > entity.Id)
            .Take(CantEntity)
            .ToListAsync<TEntity>()
            .ConfigureAwait(false);
        }

        /// <summary>
        /// Obtiene una cantidad determinada de entidades a partir de una entidad dada más la cantidad deseada mientras cumplan con una condicción determinada.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="CantEntity"></param>
        /// <param name="create_query"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual async Task<List<TEntity>> GetLoadAsync(TEntity entity, int CantEntity,
        Expression<Func<TEntity, bool>> create_query,
        params Expression<Func<TEntity, object>>[] parameters)
        {
            if (entity == null)
                throw new NullEntityException(GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.EntityIsNull));
            if (CantEntity < 0)
                throw new CantNegativeException(GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.CantIsNegative));
            //Crea la consulta para determinar las entidades relacionadas deseadas.
            IQueryable<TEntity> query = m_dbSet.AsQueryable();
            if (await query.AnyAsync().ConfigureAwait(false))
            {
                foreach (Expression<Func<TEntity, object>> entitie in parameters)
                {
                    query = query.Include(entitie);
                }
            }
            //Agrega a la consulta la cantidad de entidades que desea y partir de cual debe tomarlas.
            return await query.Where(c => c.Id > entity.Id).Where(create_query)
            .Take(CantEntity)
            .ToListAsync<TEntity>()
            .ConfigureAwait(false);
        }

        /// <summary>
        /// Devuelve una cantidad de entidades determinada a partir del Id de una entidad y la cantidad deseada.
        /// Este metodo es útil cuando tenemos una colección de entidades que todas están relacionadas con otra entidad.
        /// Así podemos obtener una cantidad determinada de las mismas, ya que el Id de está puede no estar de forma consecutiva,
        /// en la tabla de origen, por lo que usamos este método cuando tenemos todas las relaciones de una entidad.
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="list_entities"></param>
        /// <param name="CantEntity"></param>
        /// <returns></returns>
        public virtual List<TEntity> GetLoadAsync(int Id, ICollection<TEntity> list_entities, int CantEntity)
        {
            if (list_entities == null)
                throw new NullEntityException(GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.EntityIsNull));
            if (CantEntity < 0)
                throw new CantNegativeException(GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.CantIsNegative));
            //Verifica si la entidad está en la colección dada.
            var entity_reference = list_entities.FirstOrDefault(c => c.Id == Id);
            if (entity_reference == null)
                return null;
            //Crea la consulta para seleccionar a partir de cual se quiere una cantidad determinada.
            var query = from cant_entity in list_entities where cant_entity.Id > entity_reference.Id select cant_entity;
            //Ejecuta la consulta
            return query.Take(CantEntity).ToList();
        }

        /// <summary>
        /// Devuelve la ultima entidad.
        /// </summary>
        /// <returns></returns>
        public virtual async Task<TEntity> LastEntityAsync()
        {
            IQueryable<TEntity> query = m_dbSet.AsQueryable();
            //Crea la consulta para ordenar la entidad ascendentemente por el Id
            //Si no ordenamos antes de pedir la última entidad da error.
            var ordered_entity = query.OrderBy(c => c.Id);
            return await ordered_entity.LastOrDefaultAsync<TEntity>().ConfigureAwait(false);
        }

        /// <summary>
        /// Crea una consulta para aplicar a una entidad y indica las relaciones que se desean traer.
        /// Devuelve todas las entidades que cumplen con la condicción.
        /// </summary>
        /// <param name="create_query"></param>
        /// <param name="parameters_filters"></param>
        /// <returns></returns>
        public virtual async Task<List<TEntity>> WhereListEntityAsync(Expression<Func<TEntity,
        bool>> create_query,
        params Expression<Func<TEntity, object>>[] parameters_filters)
        {
            //Crea la consulta para determinar las filtros que se desean a aplicar.
            IQueryable<TEntity> query = m_dbSet.AsQueryable();
            foreach (Expression<Func<TEntity, object>> include in parameters_filters)
                query = query.Include(include);
            //Crea el filtro deseado para la consulta
            query = query.Where(create_query);
            //Ejecuta la consulta
            return await query.ToListAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Crea una consulta para aplicar a una entidad y indica las relaciones que se desean traer.
        /// Devuelve la entidad que cumple con la condicción si existe alguna otra entidad que cumple con la condicción
        /// se lanza una excepción.
        /// </summary>
        /// <param name="create_query"></param>
        /// <param name="parameters_filters"></param>
        /// <returns></returns>
        public virtual async Task<TEntity> WhereSingleEntityAsync(Expression<Func<TEntity,
        bool>> create_query,
        params Expression<Func<TEntity, object>>[] parameters_filters)
        {
            //Crea la consulta para determinar las filtros que se desean a aplicar.
            IQueryable<TEntity> query = m_dbSet.AsQueryable();
            foreach (Expression<Func<TEntity, object>> include in parameters_filters)
                query = query.Include(include);
            //Crea el filtro deseado para la consulta
            query = query.Where(create_query);
            //Ejecuta la consulta
            return await query.SingleOrDefaultAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Crea una consulta para aplicar a una entidad y indica las relaciones que se desean traer.
        /// Devuelve la primera entidad que cumpla con la condicción.
        /// </summary>
        /// <param name="create_query"></param>
        /// <param name="parameters_filters"></param>
        /// <returns></returns>
        public virtual async Task<TEntity> WhereFirstEntityAsync(Expression<Func<TEntity,
        bool>> create_query,
        params Expression<Func<TEntity, object>>[] parameters_filters)
        {
            //Crea la consulta para determinar las filtros que se desean a aplicar.
            IQueryable<TEntity> query = m_dbSet.AsQueryable();
            foreach (Expression<Func<TEntity, object>> include in parameters_filters)
                query = query.Include(include);
            //Crea el filtro deseado para la consulta
            query = query.Where(create_query);
            //Ejecuta la consulta
            return await query.FirstOrDefaultAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Verifica si una tabla contiene todas las entidades enviadas dado sus Id.
        /// </summary>
        /// <param name="ListEntityId"></param>
        /// <returns></returns>
        public virtual async Task<bool> VerifyIsContainsEntity(List<int> ListEntityId)
        {
            IQueryable<TEntity> query = m_dbSet.AsQueryable();
            foreach (int id in ListEntityId)
            {
                if (await query.FirstOrDefaultAsync(c => c.Id == id)
                .ConfigureAwait(false) != null)
                    continue;
                else
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Guarda una entidad en la base de datos.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual async Task AddEntityAsync(TEntity entity)
        {
            if (entity == null)
                throw new NullEntityException(GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.EntityIsNull));
            await m_dbSet.AddAsync(entity).ConfigureAwait(false);
        }

        /// <summary>
        /// Guarda una lista de entidades en la base de datos.
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public virtual async Task AddRangeEntityAsync(List<TEntity> entities)
        {
            if (entities == null)
                throw new NullEntityException(GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.EntityIsNull));
            await m_dbSet.AddRangeAsync(entities).ConfigureAwait(false);
        }

        /// <summary>
        /// Guarda los cambios en la base de datos.
        /// </summary>
        /// <returns></returns>
        public virtual async Task SaveChangesBDAsync()
        {
            await this._dataContext.SaveChangesAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Actualiza una lista de entidades.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual void UpdateEntity(TEntity entity)
        {
            if (entity == null)
                throw new NullEntityException(GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.EntityIsNull));
            m_dbSet.Update(entity);
        }

        /// <summary>
        /// Actualiza una entidad.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual void UpdateRangeEntity(List<TEntity> entity)
        {
            if (entity == null)
                throw new NullEntityException(GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.EntityIsNull));
            m_dbSet.UpdateRange(entity);
        }

        /// <summary>
        /// Elimina una entidad.
        /// </summary>
        /// <param name="entity"></param>
        public void RemoveEntity(TEntity entity)
        {
            if (entity == null)
                throw new NullEntityException(GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.EntityIsNull));
            m_dbSet.Remove(entity);
        }

        /// <summary>
        /// Elimina una entidad.
        /// </summary>
        /// <param name="entities"></param>
        public void RemoveRangeEntity(List<TEntity> entities)
        {
            if (entities == null)
                throw new NullEntityException(GetValueResourceFile.GetValueResourceString(GetValueResourceFile.KeyResource.EntityIsNull));
            m_dbSet.RemoveRange(entities);
        }


        #endregion

        #region Excepciones
        /// <summary>
        /// Entidad Nula.
        /// </summary>
        class NullEntityException : Exception
        {
            public NullEntityException(string message) : base(message)
            {
            }
        }
        class CantNegativeException : Exception
        {
            public CantNegativeException(string message) : base(message)
            {
            }
        }
        #endregion
    }
}
