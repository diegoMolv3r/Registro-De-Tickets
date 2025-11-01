using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace RegistroDeTickets.Service
{
    public interface IEmailService
    {
        Task EnviarEmail(string emailReceptor, string tema, string cuerpo);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration configuration;

        public EmailService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task EnviarEmail(string emailReceptor, string tema, string cuerpo)
        {
            var emailEmisor = configuration.GetSection("CONFIGURACIONES_EMAIL")["EMAIL"];
            var password = configuration.GetSection("CONFIGURACIONES_EMAIL")["PASSWORD"];
            var host = configuration.GetSection("CONFIGURACIONES_EMAIL")["HOST"];
            var puerto = Int32.Parse(configuration.GetSection("CONFIGURACIONES_EMAIL")["PUERTO"]);

            var smtpCliente = new SmtpClient(host, puerto);
            smtpCliente.EnableSsl = true;
            smtpCliente.UseDefaultCredentials = false;

            smtpCliente.Credentials = new NetworkCredential(emailEmisor, password);
            var mensaje = new MailMessage(emailEmisor!, emailReceptor, tema, cuerpo);
            mensaje.IsBodyHtml = true;

            await smtpCliente.SendMailAsync(mensaje);
        }
    }
}