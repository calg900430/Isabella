namespace Isabella.Common
{
    /// <summary>
    /// Está clase es para enviar las respuestas de la api, además de información adicional 
    /// como un mensaje de éxito o de excepción, o otros datos de importancia que necesite el usuario. 
    /// </summary>
    public class ServiceResponse<T>
    {
        public int Code { get; set; }  //Código del recurso solicitado.

        public T Data { get; set; }                  //Representa los datos del usuario

        public string Message { get; set; }          //Representa un mensaje para el usuario

        public bool Success { get; set; }            //Indica si la operacion estuvo exito o no.
    }
}
