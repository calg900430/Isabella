namespace Isabella.Common.ApiService
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using Newtonsoft.Json;

    using Isabella.Common.Dtos.Users;
  
    /// <summary>
    /// Manaja la conexion con la API
    /// </summary>
    public class ApiService : IApiService
    {
        /// <summary>
        /// Realiza el login del usuario y obtiene el Token.
        /// </summary>
        /// <param name="urlBase"></param>
        /// <param name="servicePrefix"></param>
        /// <param name="controller"></param>
        /// <param name="request_loging"></param>
        /// <returns></returns>
        /*public async Task<Response> PostAsyncIsabellaAPI<Request, Response>(string urlBase, string servicePrefix, string controller, Request request)
        {
            try
            {
                //HttpClient nos permite enviar y recibir solicitudes Http,
                //nos sirve para consumir servicios web(apis).
                //Se establece como direccion base una URI 
                using (var client = new HttpClient())
                {
                    //Establece la UIR
                    client.BaseAddress = new Uri(urlBase);
                    //Borra las solicitudes de los encabezados
                    client.DefaultRequestHeaders.Accept.Clear();
                    //Establece que los datos son en JSON
                    client.DefaultRequestHeaders.Accept
                    .Add(new System.Net.Http.Headers
                    .MediaTypeWithQualityHeaderValue("application/json"));
                    //Se crea la Url, o sea lo q va a continuación de la URI,
                    //la URI es la direccion base que no cambia.
                    var url = $"{servicePrefix}{controller}";
                    //Serializa los solicitudes a enviar
                    var request_string = JsonConvert.SerializeObject(request);
                    //La clase StringContent provee contenido Http basado en String
                    //Codifica los parametros a enviar en UTF8 y Json.
                    var content = new StringContent(request_string, Encoding.UTF8,
                    "application/json");
                    //Envia los datos a la api usando el metodo POST, los datos se envian
                    //en el Body.
                    var get_response = await client.PostAsync(url, content);
                    //Obtiene la respuesta de la peticion
                    var result = await get_response.Content.ReadAsStringAsync();
                    //Deserializa la respuesta
                    if (get_response.IsSuccessStatusCode)
                    {
                       //Deserializa la respuesta
                       return JsonConvert.DeserializeObject<Response>(result);
                    }
                }
            }
            catch (Exception ex)
            {
              
            }

        }

        public async Task<ServiceResponse<ServiceResponse<GetDataUserForLoginDto>>> LogingUserAsync(string urlBase,
        string servicePrefix, string controller, LoginUserDto request_loging)
        {
           

        }*/

      
    }
}
