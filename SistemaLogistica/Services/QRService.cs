using QRCoder;
using SistemaLogistica.Models;
using System.Drawing;
using System.Drawing.Imaging;

namespace SistemaLogistica.Services
{
    public class QRService
    {
        private readonly string _rutaQRCodes;

        public QRService(string rutaQRCodes = "QRCodes")
        {
            _rutaQRCodes = rutaQRCodes;

            if (!Directory.Exists(_rutaQRCodes))
            {
                Directory.CreateDirectory(_rutaQRCodes);
            }
        }

        public string GenerarCodigoQR(Envio envio)
        {
            try
            {
                string codigoQR = $"ENVIO:{envio.Id}";

                using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
                {
                    QRCodeData qrCodeData = qrGenerator.CreateQrCode(codigoQR, QRCodeGenerator.ECCLevel.Q);
                    using (QRCode qrCode = new QRCode(qrCodeData))
                    {
                        using (Bitmap qrCodeImage = qrCode.GetGraphic(20))
                        {
                            string rutaArchivo = Path.Combine(_rutaQRCodes, $"QR_{envio.Id}.png");
                            qrCodeImage.Save(rutaArchivo, ImageFormat.Png);

                            envio.CodigoQR = codigoQR;
                            envio.RutaQR = rutaArchivo;

                            Console.WriteLine($"✓ QR generado en: {rutaArchivo}");
                            return rutaArchivo;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Error al generar QR: {ex.Message}");
                return string.Empty;
            }
        }

        public bool ValidarCodigoQR(string codigo)
        {
            if (string.IsNullOrEmpty(codigo))
                return false;

            if (!codigo.StartsWith("ENVIO:"))
                return false;

            string numeroParte = codigo.Replace("ENVIO:", "");

            return int.TryParse(numeroParte, out _);
        }

        public int ExtraerIdDeQR(string codigo)
        {
            if (ValidarCodigoQR(codigo))
            {
                return int.Parse(codigo.Replace("ENVIO:", ""));
            }
            return -1;
        }

        public string? EscanearQRConCamara()
        {
            try
            {
                var formEscaner = new FormEscanerQR();
                formEscaner.ShowDialog();

                if (formEscaner.EscaneoExitoso && !string.IsNullOrEmpty(formEscaner.CodigoQRDetectado))
                {
                    string codigo = formEscaner.CodigoQRDetectado;
                    formEscaner.Dispose();
                    return codigo;
                }

                formEscaner.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al escanear QR: {ex.Message}");
            }

            return null;
        }
    }
}