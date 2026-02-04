using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using SistemaLogistica.Models;

namespace SistemaLogistica.Services
{
    public class SMSService
    {
        private readonly Configuracion _config;

        public SMSService(Configuracion config)
        {
            _config = config;
            TwilioClient.Init(_config.TwilioAccountSid, _config.TwilioAuthToken);
        }

        public async Task<bool> EnviarSMSRegistro(Envio envio)
        {
            try
            {
                string mensaje = $"{_config.NombreEmpresa}: Envío #{envio.Id} registrado. Código: {envio.CodigoQR}";

                var message = await MessageResource.CreateAsync(
                    body: mensaje,
                    from: new PhoneNumber(_config.TwilioPhoneNumber),
                    to: new PhoneNumber(envio.Telefono)
                );

                Console.WriteLine($"✓ SMS enviado a {envio.Telefono}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Error SMS: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> EnviarSMSEntrega(Envio envio)
        {
            try
            {
                string mensaje = $"✓ {_config.NombreEmpresa}: Paquete #{envio.Id} ENTREGADO el {envio.FechaEntrega:dd/MM/yyyy}";

                var message = await MessageResource.CreateAsync(
                    body: mensaje,
                    from: new PhoneNumber(_config.TwilioPhoneNumber),
                    to: new PhoneNumber(envio.Telefono)
                );

                Console.WriteLine($"✓ SMS enviado a {envio.Telefono}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Error SMS: {ex.Message}");
                return false;
            }
        }
    }
}