namespace Duma.API.Repositorys
{
    /// <summary>
    /// Repositorio para el manejo de el envio de correos.
    /// </summary>
    public interface IMailRepository
    {
       /// <summary>
       /// Envia un correo a un destinatario.
       /// </summary>
       /// <param name="destiny"></param>
       /// <param name="subject"></param>
       /// <param name="body_message"></param>
       public void SendMail(string destiny, string subject, string body_message);
    }
}
