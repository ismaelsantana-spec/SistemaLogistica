using iTextSharp.text;
using iTextSharp.text.pdf;
using SistemaLogistica.Models;
using System.IO;

namespace SistemaLogistica.Services
{
    public class PDFService
    {
        private readonly string _rutaFacturas;
        private readonly Configuracion _config;

        public PDFService(Configuracion config, string rutaFacturas = "Facturas")
        {
            _rutaFacturas = rutaFacturas;
            _config = config;

            if (!Directory.Exists(_rutaFacturas))
            {
                Directory.CreateDirectory(_rutaFacturas);
            }
        }

        public string GenerarFactura(Envio envio)
        {
            string rutaArchivo = Path.Combine(_rutaFacturas, $"Factura_{envio.Id}.pdf");

            using (FileStream fs = new FileStream(rutaArchivo, FileMode.Create))
            {
                Document document = new Document(PageSize.A4);
                PdfWriter.GetInstance(document, fs);

                document.Open();

                // Fuentes usando iTextSharp
                iTextSharp.text.Font fontTitulo = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18f);
                iTextSharp.text.Font fontSubtitulo = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12f);
                iTextSharp.text.Font fontNormal = FontFactory.GetFont(FontFactory.HELVETICA, 10f);
                iTextSharp.text.Font fontPequeno = FontFactory.GetFont(FontFactory.HELVETICA, 8f);

                // Título
                Paragraph titulo = new Paragraph(_config.NombreEmpresa, fontTitulo);
                titulo.Alignment = Element.ALIGN_CENTER;
                document.Add(titulo);

                // Dirección
                Paragraph direccion = new Paragraph(_config.DireccionEmpresa, fontPequeno);
                direccion.Alignment = Element.ALIGN_CENTER;
                document.Add(direccion);

                // RUC y teléfono
                Paragraph ruc = new Paragraph($"RUC: {_config.RucEmpresa} | Tel: {_config.TelefonoEmpresa}", fontPequeno);
                ruc.Alignment = Element.ALIGN_CENTER;
                document.Add(ruc);

                document.Add(new Paragraph(" "));

                // Número de factura
                Paragraph numeroFactura = new Paragraph($"FACTURA DE ENVÍO #{envio.Id:D6}", fontSubtitulo);
                numeroFactura.Alignment = Element.ALIGN_CENTER;
                document.Add(numeroFactura);

                // Fecha
                Paragraph fecha = new Paragraph($"Fecha: {envio.FechaRegistro:dd/MM/yyyy HH:mm}", fontNormal);
                fecha.Alignment = Element.ALIGN_CENTER;
                document.Add(fecha);

                document.Add(new Paragraph(" "));

                // Datos del cliente
                document.Add(new Paragraph("DATOS DEL CLIENTE", fontSubtitulo));
                document.Add(new Paragraph($"Cliente: {envio.Cliente}", fontNormal));
                document.Add(new Paragraph($"Teléfono: {envio.Telefono}", fontNormal));
                document.Add(new Paragraph($"Email: {envio.Email}", fontNormal));
                document.Add(new Paragraph($"Dirección: {envio.Direccion}", fontNormal));
                document.Add(new Paragraph($"Ciudad: {envio.Ciudad}", fontNormal));
                document.Add(new Paragraph(" "));

                // Detalles del envío
                document.Add(new Paragraph("DETALLES DEL ENVÍO", fontSubtitulo));

                // Crear tabla
                PdfPTable table = new PdfPTable(3);
                table.WidthPercentage = 100f;
                float[] anchos = new float[] { 2f, 1f, 1f };
                table.SetWidths(anchos);

                // Color gris claro
                BaseColor grisClaro = new BaseColor(211, 211, 211);

                // Encabezados de tabla
                PdfPCell celda1 = new PdfPCell(new Phrase("Descripción", fontSubtitulo));
                celda1.BackgroundColor = grisClaro;
                table.AddCell(celda1);

                PdfPCell celda2 = new PdfPCell(new Phrase("Peso (kg)", fontSubtitulo));
                celda2.BackgroundColor = grisClaro;
                table.AddCell(celda2);

                PdfPCell celda3 = new PdfPCell(new Phrase("Costo", fontSubtitulo));
                celda3.BackgroundColor = grisClaro;
                table.AddCell(celda3);

                // Datos de la tabla
                table.AddCell(new Phrase($"Envío de paquete - {envio.Ciudad}", fontNormal));
                table.AddCell(new Phrase(envio.Peso.ToString("F2"), fontNormal));
                table.AddCell(new Phrase($"${envio.Costo:F2}", fontNormal));

                document.Add(table);
                document.Add(new Paragraph(" "));

                // Total
                Paragraph total = new Paragraph($"TOTAL A PAGAR: ${envio.Costo:F2}", fontSubtitulo);
                total.Alignment = Element.ALIGN_RIGHT;
                document.Add(total);
                document.Add(new Paragraph(" "));

                // Información de entrega
                document.Add(new Paragraph("INFORMACIÓN DE ENTREGA", fontSubtitulo));
                document.Add(new Paragraph($"Código QR: {envio.CodigoQR}", fontNormal));

                string estadoTexto = envio.Estado.ToString();
                document.Add(new Paragraph($"Estado: {estadoTexto}", fontNormal));

                if (envio.FechaEntrega.HasValue)
                {
                    document.Add(new Paragraph($"Fecha de entrega: {envio.FechaEntrega.Value:dd/MM/yyyy HH:mm}", fontNormal));
                }

                document.Add(new Paragraph(" "));

                // Footer
                Paragraph footer = new Paragraph("Gracias por confiar en nuestros servicios", fontPequeno);
                footer.Alignment = Element.ALIGN_CENTER;
                document.Add(footer);

                document.Close();
            }

            envio.RutaFactura = rutaArchivo;
            return rutaArchivo;
        }
    }
}