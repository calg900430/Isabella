namespace Isabella.Common.ApiService
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;

    using Isabella.Common.Dtos.Users;

    public interface IApiService
    {
        /// <summary>
        /// Envia una solicitud Post con solicitudes y devuelve una respuesta.
        /// </summary>
        /// <param name="urlBase"></param>
        /// <param name="servicePrefix"></param>
        /// <param name="controller"></param>
        /// <returns></returns>
        /*Task<Response> PostAsyncIsabellaAPI<Request,Response>(string urlBase, 
        string servicePrefix, string controller, Request response);*/
    }
}
