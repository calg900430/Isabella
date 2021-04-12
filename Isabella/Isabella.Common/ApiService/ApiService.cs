using System;
using System.Collections.Generic;
using System.Text;

namespace Isabella.Common.ApiService
{
    public class ApiService
    {
        /*public async Task<Response> RegisterUserAsync(
          string urlBase,
          string servicePrefix,
          string controller,
          NewUserRequest newUserRequest)
        {
            try
            {
                var request = JsonConvert.SerializeObject(newUserRequest);
                var content = new StringContent(request, Encoding.UTF8, "application/json");
                var client = new HttpClient
                {
                    BaseAddress = new Uri(urlBase)
                };

                var url = $"{servicePrefix}{controller}";
                var response = await client.PostAsync(url, content);
                var answer = await response.Content.ReadAsStringAsync();
                var obj = JsonConvert.DeserializeObject<Response>(answer);
                return obj;
            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = ex.Message,
                };
            }
        }*/
    }
}
