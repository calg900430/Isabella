namespace Isabella.Common.RepositorysDtos
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Common;
    using Common.Dtos.Aggregate;

    /// <summary>
    /// Repositorio para el manejo de los Dtos de los agregados.
    /// </summary>
    public interface IAggregateRepositoryDto
    {
        /// <summary>
        /// Agrega un nuevo agregado.
        /// </summary>
        /// <param name="addAggregate"></param>
        /// <returns></returns>
        Task<ServiceResponse<bool>> AddAggregateAsync(AddAggregateDto addAggregate);

        /// <summary>
        /// Actualiza un agregado.
        /// </summary>
        /// <param name="updateAggregate"></param>
        /// <returns></returns>
        Task<ServiceResponse<GetAggregateDto>> UpdateAggregateAsync(UpdateAggregateDto updateAggregate);

        /// <summary>
        /// Obtiene un agregado dado su Id.
        /// </summary>
        /// <param name="AggregateId"></param>
        /// <returns></returns>
        Task<ServiceResponse<GetAggregateDto>> GetAggregateForIdAsync(int AggregateId);

        /// <summary>
        /// Obtiene todos los agregado disponibles en la base de datos.
        /// </summary>
        /// <returns></returns>
        Task<ServiceResponse<List<GetAggregateDto>>> GetAllAggregateAsync();

        /// <summary>
        /// Obtiene el Id del último agregado.
        /// </summary>
        /// <returns></returns>
        Task<ServiceResponse<int>> GetIdOfLastAggregateAsync();

        /// <summary>
        /// Obtiene la imagen de un agregado.
        /// </summary>
        /// <param name="AggregateId"></param>
        /// <param name="ImageId"></param>
        /// <returns></returns>
        Task<ServiceResponse<GetImageAggregateDto>> GetImageAggregateForIdAsync(int AggregateId, int ImageId);

        /// <summary>
        /// Agrega una imagen a un agregado.
        /// </summary>
        /// <param name="addImageAggregate"></param>
        /// <returns></returns>
        Task<ServiceResponse<bool>> AddImageAggregateAsync(AddImageAggregateDto addImageAggregate);

        /// <summary>
        /// Borra una imagen de un agregado.
        /// </summary>
        /// <param name="AggregateId"></param>
        /// <param name="ImageId"></param>
        /// <returns></returns>
        Task<ServiceResponse<bool>> DeleteImageAggregateAsync(int AggregateId, int ImageId);

        /// <summary>
        /// Habilita o deshabilita un agregado.
        /// </summary>
        /// <param name="AggregateId"></param>
        /// <param name="enable"></param>
        /// <returns></returns>
        Task<ServiceResponse<bool>> EnableAggregateAsync(int AggregateId, bool enable);

        /// <summary>
        /// Obtiene una cantidad determinada de agregados dado un agregado de referencia y la cantidad.
        /// </summary>
        /// <param name="AggregateId"></param>
        /// <param name="cantAggregate"></param>
        /// <returns></returns>
        Task<ServiceResponse<List<GetAggregateDto>>> GetCantAggregateAsync(int AggregateId, int cantAggregate);

        /// <summary>
        /// Devuelve una lista con los Id de todas las imagenes del agregado.
        /// </summary>
        /// <param name="AggregateId"></param>
        /// <returns></returns>
        Task<ServiceResponse<List<int>>> GetListIdOfImageAggregateAsync(int AggregateId);

        /// <summary>
        /// Devuelve todas las imagenes de un agregado.
        /// </summary>
        /// <param name="AggregateId"></param>
        /// <returns></returns>
        Task<ServiceResponse<List<GetImageAggregateDto>>> GetAllImageAggregateAsync(int AggregateId);

        /// <summary>
        /// Devuelve una cantidad de imagenes de un agregado a partir de una imagen de referencia.
        /// </summary>
        /// <param name="AggregateId"></param>
        /// <param name="Image"></param>
        /// <param name="cantImage"></param>
        /// <returns></returns>
        Task<ServiceResponse<List<GetImageAggregateDto>>> GetCantImageAggregateAsync(int AggregateId, int Image, int cantImage);

        /// <summary>
        /// Obtiene un agregado dado su Id si el mismo está disponible.
        /// </summary>
        /// <param name="AggregateId"></param>
        /// <returns></returns>
        Task<ServiceResponse<GetAggregateDto>> GetAggregateIsAvailableForIdAsync(int AggregateId);

        /// <summary>
        /// Obtiene todos los agregados disponibles en la base de datos.
        /// </summary>
        /// <returns></returns>
        Task<ServiceResponse<List<GetAggregateDto>>> GetAllAggregateIsAvailableAsync();

        /// <summary>
        /// Obtiene una cantidad determinada de agregados disponibles dado un agregado de referencia y la cantidad.
        /// </summary>
        /// <param name="AggregateId"></param>
        /// <param name="cantAggregate"></param>
        /// <returns></returns>
        Task<ServiceResponse<List<GetAggregateDto>>> GetCantAggregateIsAvailableAsync(int AggregateId, int cantAggregate);

        /// <summary>
        /// Borra un agregado.
        /// </summary>
        /// <param name="AggregateId"></param>
        /// <returns></returns>
        Task<ServiceResponse<bool>> DeleteAggregateAsync(int AggregateId);

    }
}
