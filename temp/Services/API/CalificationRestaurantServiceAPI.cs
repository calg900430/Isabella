namespace Isabella.Web.Services.API
{
    using System;
    using System.Threading.Tasks;
    using AutoMapper;
    using Microsoft.EntityFrameworkCore;
    using Common;
    using Common.Dtos.CalificationRestaurant;
    using Models.Entities;
    using Repositorys.API;
    using Data;

    /// <summary>
    /// Servicio para la calificación del restaurante.
    /// </summary>
    public class CalificationRestaurantServiceAPI:GenericService<CalificationRestaurant>, ICalificationRestaurantRepositoryAPI
    {
        private readonly DataContext _dataContext;
        private readonly IMapper _mapper;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataContext"></param>
        /// <param name="mapper"></param>
        public CalificationRestaurantServiceAPI(DataContext dataContext, IMapper mapper) : base(dataContext)
        {
            this._dataContext = dataContext;
            this._mapper = mapper;
        }

        /// <summary>
        /// Agrega una calificación acerca del restaurante.
        /// </summary>
        /// <param name="addCalificationRestaurant"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<bool>> AddCalificationForRestaurantAsync(AddCalificationRestaurantDto addCalificationRestaurant)
        {
            ServiceResponse<bool> serviceResponse = new ServiceResponse<bool>();
            try
            {
                if (addCalificationRestaurant == null)
                {
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = "Debe enviar los datos necesarios para agregar una calificación acerca del restaurante.";
                    return serviceResponse;
                }
                //Verifica que el usuario este registrado en la base de datos.
                var user = await this._dataContext.Users.
                FirstOrDefaultAsync(c => c.CodeUser == Guid.Parse(addCalificationRestaurant.CodeUser));
                if (user == null)
                {
                    serviceResponse.Data = false;
                    serviceResponse.Success = false;
                    serviceResponse.Message = "El usuario no se encuentra registrado en la base de datos.";
                    return serviceResponse;
                }
                //Mapea de AddCalificationRestaurantDto a CalificationRestaurant
                var calificationrestaurant = this._mapper.Map<CalificationRestaurant>(addCalificationRestaurant);
                //Asigna los campos restantes
                calificationrestaurant.DateCreated = DateTime.UtcNow;
                calificationrestaurant.User = user;
                //Agrega la calificación del restaurante a la base de datos
                await this.CreateAsync(calificationrestaurant).ConfigureAwait(false);
                serviceResponse.Data = true;
                serviceResponse.Success = true;
                serviceResponse.Message = "Se ha agregado su calificación acerca del restaurante.";
            }
            catch (SystemException)
            {
                serviceResponse.Data = false;
                serviceResponse.Success = false;
                serviceResponse.Message = "No se encuentra el usuario en la base de datos.";
                return serviceResponse;
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }
            return serviceResponse;
        }
    }
}
