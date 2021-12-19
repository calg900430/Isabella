namespace Isabella.Web.Helpers
{
    using System;
    using Microsoft.Extensions.Configuration;

    using MimeKit;
    using MailKit.Net.Smtp;
   
    /// <summary>
    /// Servicio para el envio de correos electronicos.
    /// </summary>
    public class MailHelper
    {
        private readonly IConfiguration _configuration;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="configuration"></param>
        public MailHelper(IConfiguration configuration)
        {
            this._configuration = configuration;         
        }

        /// <summary>
        /// Envia correos a un destinatario.
        /// </summary>
        /// <param name="destiny"></param>
        /// <param name="subject"></param>
        /// <param name="body_message"></param>
        public bool SendMail(string destiny, string subject, string body_message)
        {
            try
            {
                //Obtiene el correo de la aplicación
                var from = this._configuration["Mail:From"];
                //Obtiene el servidor smtp de nuestra cuenta de correo para nuestra aplicación
                var smtp = this._configuration["Mail:Smtp"];
                //Obtiene el puesto del servidor smtp.
                var port = this._configuration["Mail:Port"];
                //Obtiene el password de nuestra cuenta de correo
                var password = this._configuration["Mail:Password"];

                //Crea una nueva instancia de la clase MimeMessage
                //Nos sirve para crear un correo electrónico
                var message = new MimeMessage();

                //Crea un nuevo enlace de correo entre el origen y el destinatario
                message.From.Add(new MailboxAddress(from));
                message.To.Add(new MailboxAddress(destiny));

                //Le asigna un asunto a nuestro correo
                message.Subject = subject;

                //Crea una instancia para un configurador de cuerpos de correo
                var bodyBuilder = new BodyBuilder();

                //Crea el cuerpo de nuestro correo con formato HTML
                //Esto permite que nuestro cuerpo de mensaje pueda tener 
                //etiquetas HTML, estilos,ect
                //bodyBuilder.HtmlBody = body_message;

                //Crea el cuerpo de nuestro correo con formato Html.
                bodyBuilder.HtmlBody = body_message;

                //Construye el cuerpo mensaje con la informacion disponible 
                //del cuerpo del mensaje 
                message.Body = bodyBuilder.ToMessageBody();

                //Creamos una instancia de SmtpClient, es la que me permite enviar correos.
                using (var client = new SmtpClient())
                {
                    //Nos conectamos al servidor le indicamos que no queremos SSL
                    client.Connect(smtp, int.Parse(port), false);
                    //Se autentica al Servidor de Correo donde tengamos alojados el correo 
                    //de la app, en este caso es Gmail
                    client.Authenticate(from, password);
                    //Envia el mensaje al destinatario
                    client.Send(message);
                    //Nos desconecta.
                    client.Disconnect(true);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
