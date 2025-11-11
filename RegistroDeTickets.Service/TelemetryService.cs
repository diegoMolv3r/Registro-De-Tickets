using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using RegistroDeTickets.Data.Entidades;

namespace RegistroDeTickets.Service
{
    public interface ITelemetryService
    {
        void RegistrarEvento(string nombreEvento, Usuario? usuario);
    }
    public class TelemetryService : ITelemetryService
    {
        private readonly TelemetryClient _telemetryClient;
        public TelemetryService (TelemetryClient telemetryClient)
        {
            _telemetryClient = telemetryClient;
        }

        // Las 'propiedades' son para la informacion adicional que se quiera agregar al evento
        public void RegistrarEvento(string nombreEvento, Usuario? usuario)
        {
            var propiedades = new Dictionary<string, string>();
            
            if (usuario != null)
            {
                propiedades.Add("UsuarioId", usuario.Id.ToString());
                propiedades.Add("Email", usuario.Email);
                propiedades.Add("Username", usuario.UserName);
                _telemetryClient.TrackEvent(nombreEvento, propiedades);
                _telemetryClient.Flush();
            }
            else
            {

                propiedades.Add("UsuarioId", "No existe");
                _telemetryClient.TrackEvent(nombreEvento, propiedades);
                _telemetryClient.Flush();
            }

            
        }

       

    }
}
