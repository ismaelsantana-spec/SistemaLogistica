using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using SistemaLogistica.Models;

namespace SistemaLogistica.Services
{
    public class EmailService
    {
        private readonly Configuracion _config;

        public EmailService(Configuracion config)
        {
            _config = config;
        }

        public async Task<bool> EnviarFacturaPorEmail(Envio envio)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_config.NombreEmpresa, _config.EmailFrom));
                message.To.Add(new MailboxAddress(envio.Cliente, envio.Email));
                message.Subject = $"Factura de envío #{envio.Id:D6} - {_config.NombreEmpresa}";

                var builder = new BodyBuilder();
                builder.HtmlBody = $@"
                    <html>
                    <body style='font-family: Arial, sans-serif;'>
                        <h2>¡Hola {envio.Cliente}!</h2>
                        <p>Gracias por confiar en <strong>{_config.NombreEmpresa}</strong>.</p>
                        <p>Tu envío #{envio.Id} ha sido registrado.</p>
                        <p><strong>Código QR:</strong> {envio.CodigoQR}</p>
                        <p><strong>Destino:</strong> {envio.Direccion}, {envio.Ciudad}</p>
                        <p><strong>Costo:</strong> ${envio.Costo:F2}</p>
                    </body>
                    </html>
                ";

                if (!string.IsNullOrEmpty(envio.RutaFactura) && File.Exists(envio.RutaFactura))
                {
                    builder.Attachments.Add(envio.RutaFactura);
                }

                message.Body = builder.ToMessageBody();

                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync(_config.SmtpHost, _config.SmtpPort, SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync(_config.EmailFrom, _config.EmailPassword);
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }

                Console.WriteLine($"✓ Email enviado a {envio.Email}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Error al enviar email: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> EnviarNotificacionEntrega(Envio envio)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_config.NombreEmpresa, _config.EmailFrom));
                message.To.Add(new MailboxAddress(envio.Cliente, envio.Email));
                message.Subject = $"¡Paquete entregado! - Envío #{envio.Id:D6}";

                var builder = new BodyBuilder();
                builder.HtmlBody = $@"
                    <html>
                    <body>
                        <h2>✓ ¡Entrega Exitosa!</h2>
                        <p>Tu paquete #{envio.Id} fue entregado el {envio.FechaEntrega:dd/MM/yyyy} a las {envio.FechaEntrega:HH:mm}</p>
                    </body>
                    </html>
                ";

                message.Body = builder.ToMessageBody();

                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync(_config.SmtpHost, _config.SmtpPort, SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync(_config.EmailFrom, _config.EmailPassword);
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }

                Console.WriteLine($"✓ Notificación enviada a {envio.Email}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Error: {ex.Message}");
                return false;
            }
        }
    }
}