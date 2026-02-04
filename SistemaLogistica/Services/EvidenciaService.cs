using System.Drawing;
using System.Drawing.Imaging;
using SistemaLogistica.Models;

namespace SistemaLogistica.Services
{
    public class EvidenciaService
    {
        private readonly string _rutaEvidencias;

        public EvidenciaService(string rutaEvidencias = "Images")
        {
            _rutaEvidencias = rutaEvidencias;

            if (!Directory.Exists(_rutaEvidencias))
            {
                Directory.CreateDirectory(_rutaEvidencias);
            }
        }

        public string CapturarEvidencia(Envio envio)
        {
            try
            {
                Console.WriteLine("\n📸 Abriendo cámara para capturar evidencia...");

                Bitmap? fotoCapturada = null;

                using (var formCaptura = new FormCapturarFoto())
                {
                    formCaptura.ShowDialog();

                    if (formCaptura.CapturaExitosa && formCaptura.FotoCapturada != null)
                    {
                        fotoCapturada = (Bitmap)formCaptura.FotoCapturada.Clone();
                    }
                }

                if (fotoCapturada != null)
                {
                    string rutaArchivo = Path.Combine(_rutaEvidencias, $"evidencia_{envio.Id}.jpg");

                    // Agregar información a la imagen
                    using (Graphics graphics = Graphics.FromImage(fotoCapturada))
                    {
                        Font font = new Font("Arial", 12, FontStyle.Bold);

                        // Fondo semi-transparente para el texto
                        Rectangle rectFondo = new Rectangle(10, 10, 400, 120);
                        using (SolidBrush brushFondo = new SolidBrush(Color.FromArgb(180, 0, 0, 0)))
                        {
                            graphics.FillRectangle(brushFondo, rectFondo);
                        }

                        // Texto con información
                        int y = 20;
                        graphics.DrawString($"EVIDENCIA DE ENTREGA", font, Brushes.White, new PointF(20, y));
                        y += 25;
                        graphics.DrawString($"Envío: #{envio.Id}", font, Brushes.Yellow, new PointF(20, y));
                        y += 25;
                        graphics.DrawString($"Cliente: {envio.Cliente}", font, Brushes.LightGreen, new PointF(20, y));
                        y += 25;
                        graphics.DrawString($"Fecha: {DateTime.Now:dd/MM/yyyy HH:mm:ss}", font, Brushes.White, new PointF(20, y));
                    }

                    // Guardar la imagen
                    fotoCapturada.Save(rutaArchivo, ImageFormat.Jpeg);
                    fotoCapturada.Dispose();

                    envio.RutaEvidencia = rutaArchivo;
                    Console.WriteLine($"✓ Evidencia guardada: {rutaArchivo}");
                    return rutaArchivo;
                }
                else
                {
                    Console.WriteLine("✗ No se capturó ninguna foto");
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Error: {ex.Message}");
                return string.Empty;
            }
        }
    }
}