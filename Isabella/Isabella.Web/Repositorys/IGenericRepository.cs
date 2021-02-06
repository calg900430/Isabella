namespace Duma.API.Repositorys
{
    using System.Threading.Tasks;

    /// <summary>
    /// Repositorio Generico.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IGenericRepository<T>  where T: class
    {
        /// <summary>
        /// Accede a un elemento por su Id.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        Task<T> GetByIdAsync(int Id);

        /// <summary>
        /// Crea un elemento en el sistema y devuelve el elemento creado.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<T> CreateAsync(T entity);


        /// <summary>
        /// Actualiza un elemento en el sistema y devuelve el elemento actualizado.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<T> UpdateAsync(T entity);


        /// <summary>
        /// Borra un elemento del sistema.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task DeleteAsync(T entity);

        /// <summary>
        /// Verifica si un elemento existe en el sistema.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        Task<bool> ExistAsync(int Id);

        /// <summary>
        ///  Guarda los cambios hechos en el sistema.
        /// </summary>
        /// <returns></returns>
        Task<bool> SaveAllAsync();
    }
}
