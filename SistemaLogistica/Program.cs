using SistemaLogistica.Models;
using SistemaLogistica.Data;
using SistemaLogistica.Services;

namespace SistemaLogistica
{
    class Program
    {
        private static EnvioRepository _repository = null!;
        private static QRService _qrService = null!;
        private static PDFService _pdfService = null!;
        private static EmailService _emailService = null!;
        private static SMSService _smsService = null!;
        private static EvidenciaService _evidenciaService = null!;
        private static Configuracion _config = null!;

        static async Task Main(string[] args)
        {
            InicializarServicios();
            await MostrarMenuPrincipal();
        }

        private static void InicializarServicios()
        {
            _config = new Configuracion();
            _repository = new EnvioRepository();
            _qrService = new QRService();
            _pdfService = new PDFService(_config);
            _emailService = new EmailService(_config);
            _smsService = new SMSService(_config);
            _evidenciaService = new EvidenciaService();

            Console.WriteLine($"✓ Directorio de trabajo: {Directory.GetCurrentDirectory()}");
        }

        private static async Task MostrarMenuPrincipal()
        {
            bool salir = false;

            while (!salir)
            {
                Console.Clear();
                Console.WriteLine("=================================================");
                Console.WriteLine("   SISTEMA DE LOGÍSTICA Y ENTREGA DE PAQUETERÍA");
                Console.WriteLine("=================================================");
                Console.WriteLine();
                Console.WriteLine("1. Registrar envío (logística)");
                Console.WriteLine("2. Listar envíos y estados");
                Console.WriteLine("3. Buscar envío");
                Console.WriteLine("4. Gestión de entrega (repartidor)");
                Console.WriteLine("5. Eliminar envío");
                Console.WriteLine("6. Salir");
                Console.WriteLine();
                Console.Write("Seleccione una opción: ");

                string? opcion = Console.ReadLine();

                switch (opcion)
                {
                    case "1":
                        await RegistrarEnvio();
                        break;
                    case "2":
                        ListarEnvios();
                        break;
                    case "3":
                        BuscarEnvio();
                        break;
                    case "4":
                        await ModuloEntrega();
                        break;
                    case "5":
                        EliminarEnvio();
                        break;
                    case "6":
                        salir = true;
                        Console.WriteLine("\n👋 ¡Hasta pronto!");
                        break;
                    default:
                        Console.WriteLine("\n❌ Opción inválida");
                        break;
                }

                if (!salir && opcion != "6")
                {
                    Console.WriteLine("\nPresione cualquier tecla para continuar...");
                    Console.ReadKey();
                }
            }
        }

        private static async Task RegistrarEnvio()
        {
            Console.Clear();
            Console.WriteLine("--- REGISTRO DE ENVÍO ---");
            Console.WriteLine();

            var envio = new Envio
            {
                Id = _repository.ObtenerSiguienteId()
            };

            Console.Write("Cliente: ");
            envio.Cliente = Console.ReadLine() ?? "";

            Console.Write("Teléfono: ");
            envio.Telefono = Console.ReadLine() ?? "";

            Console.Write("Email: ");
            envio.Email = Console.ReadLine() ?? "";

            Console.Write("Dirección de entrega: ");
            envio.Direccion = Console.ReadLine() ?? "";

            Console.Write("Ciudad: ");
            envio.Ciudad = Console.ReadLine() ?? "";

            Console.Write("Peso (kg): ");
            if (double.TryParse(Console.ReadLine(), out double peso))
            {
                envio.Peso = peso;
            }

            Console.Write("Costo: $");
            if (decimal.TryParse(Console.ReadLine(), out decimal costo))
            {
                envio.Costo = costo;
            }

            Console.WriteLine();
            Console.WriteLine("⏳ Generando código QR...");

            // Generar QR
            string rutaQR = _qrService.GenerarCodigoQR(envio);

            if (!string.IsNullOrEmpty(rutaQR))
            {
                Console.WriteLine($"✔ Código QR generado: {envio.CodigoQR}");
                Console.WriteLine($"✔ Archivo guardado en: {Path.GetFullPath(rutaQR)}");
            }
            else
            {
                Console.WriteLine("✗ Error al generar código QR");
            }

            // Generar factura
            Console.WriteLine("⏳ Generando factura PDF...");
            string rutaFactura = _pdfService.GenerarFactura(envio);
            Console.WriteLine($"✔ Factura generada: {Path.GetFullPath(rutaFactura)}");

            // Guardar en repositorio
            _repository.Agregar(envio);

            // Enviar email con factura
            if (!string.IsNullOrEmpty(envio.Email))
            {
                Console.WriteLine();
                Console.WriteLine("📧 Enviando factura por email...");

                try
                {
                    bool emailEnviado = await _emailService.EnviarFacturaPorEmail(envio);

                    if (emailEnviado)
                    {
                        Console.WriteLine("✔ Email enviado correctamente");
                    }
                    else
                    {
                        Console.WriteLine("✗ No se pudo enviar el email (verifica la configuración)");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"✗ Error al enviar email: {ex.Message}");
                    Console.WriteLine("  Tip: Configura tu email en Models/Configuracion.cs");
                }
            }

            Console.WriteLine();
            Console.WriteLine("✔ Envío registrado correctamente");
            Console.WriteLine("✔ Estado inicial: EN TRÁNSITO");
            Console.WriteLine("✔ Paquete listo para despacho");
        }

        private static void ListarEnvios()
        {
            Console.Clear();
            Console.WriteLine("--- CONTROL DE ENVÍOS ---");
            Console.WriteLine();

            var envios = _repository.ObtenerTodos();

            if (envios.Count == 0)
            {
                Console.WriteLine("No hay envíos registrados.");
                return;
            }

            Console.WriteLine("ID   Cliente            Ciudad          Estado");
            Console.WriteLine("-----------------------------------------------------");

            foreach (var envio in envios)
            {
                string estadoTexto = envio.Estado switch
                {
                    EstadoEnvio.EnTransito => "En tránsito",
                    EstadoEnvio.EnReparto => "En reparto",
                    EstadoEnvio.Entregado => "Entregado",
                    EstadoEnvio.Cancelado => "Cancelado",
                    _ => "Desconocido"
                };

                Console.WriteLine($"{envio.Id,-4} {TruncateString(envio.Cliente, 18),-18} {TruncateString(envio.Ciudad, 15),-15} {estadoTexto}");
            }

            Console.WriteLine();
            Console.WriteLine($"Total de envíos: {envios.Count}");
        }

        private static void BuscarEnvio()
        {
            Console.Clear();
            Console.Write("Ingrese ID del envío: ");

            if (int.TryParse(Console.ReadLine(), out int id))
            {
                var envio = _repository.BuscarPorId(id);

                if (envio != null)
                {
                    Console.WriteLine();
                    Console.WriteLine("--- SEGUIMIENTO LOGÍSTICO ---");
                    Console.WriteLine($"Cliente: {envio.Cliente}");
                    Console.WriteLine($"Destino: {envio.Direccion} - {envio.Ciudad}");
                    Console.WriteLine($"Estado: {ObtenerTextoEstado(envio.Estado)}");
                    Console.WriteLine($"QR asignado: {(!string.IsNullOrEmpty(envio.CodigoQR) ? "✔" : "❌")}");
                    Console.WriteLine($"Evidencia: {(!string.IsNullOrEmpty(envio.RutaEvidencia) ? "✔" : "❌ (pendiente)")}");

                    if (!string.IsNullOrEmpty(envio.RutaQR))
                    {
                        Console.WriteLine($"Ruta QR: {Path.GetFullPath(envio.RutaQR)}");
                    }
                }
                else
                {
                    Console.WriteLine($"\n❌ No se encontró un envío con ID {id}");
                }
            }
            else
            {
                Console.WriteLine("\n❌ ID inválido");
            }
        }

        private static async Task ModuloEntrega()
        {
            bool volverMenu = false;

            while (!volverMenu)
            {
                Console.Clear();
                Console.WriteLine("--- MÓDULO DE ENTREGA ---");
                Console.WriteLine();
                Console.WriteLine("1. Escanear código QR del paquete");
                Console.WriteLine("2. Ver datos del envío");
                Console.WriteLine("3. Tomar foto de evidencia");
                Console.WriteLine("4. Confirmar entrega");
                Console.WriteLine("5. Volver al menú");
                Console.WriteLine();
                Console.Write("Seleccione una opción: ");

                string? opcion = Console.ReadLine();

                switch (opcion)
                {
                    case "1":
                        await EscanearQR();
                        break;
                    case "2":
                        VerDatosEnvio();
                        break;
                    case "3":
                        TomarFotoEvidencia();
                        break;
                    case "4":
                        await ConfirmarEntrega();
                        break;
                    case "5":
                        volverMenu = true;
                        break;
                    default:
                        Console.WriteLine("\n❌ Opción inválida");
                        Console.WriteLine("\nPresione cualquier tecla para continuar...");
                        Console.ReadKey();
                        break;
                }
            }
        }

        private static Envio? _envioActual = null;

        private static async Task EscanearQR()
        {
            Console.Clear();
            Console.WriteLine("📲 OPCIONES DE ESCANEO");
            Console.WriteLine();
            Console.WriteLine("1. 📸 Escanear con cámara web (REAL)");
            Console.WriteLine("2. ⌨️  Ingresar código manualmente");
            Console.WriteLine();
            Console.Write("Seleccione opción: ");

            string? opcion = Console.ReadLine();
            string? codigoQR = null;

            if (opcion == "1")
            {
                Console.WriteLine();
                Console.WriteLine("📸 Abriendo cámara web...");
                Console.WriteLine("⏳ Acerque el código QR a la cámara...");

                await Task.Delay(1000);

                // Escanear con cámara REAL
                codigoQR = _qrService.EscanearQRConCamara();

                if (string.IsNullOrEmpty(codigoQR))
                {
                    Console.WriteLine("\n❌ Escaneo cancelado o sin resultado");
                    Console.WriteLine("\nPresione cualquier tecla para continuar...");
                    Console.ReadKey();
                    return;
                }
            }
            else if (opcion == "2")
            {
                Console.WriteLine();
                Console.Write("Ingrese el código QR: ");
                codigoQR = Console.ReadLine();
            }
            else
            {
                Console.WriteLine("\n❌ Opción inválida");
                Console.WriteLine("\nPresione cualquier tecla para continuar...");
                Console.ReadKey();
                return;
            }

            if (string.IsNullOrEmpty(codigoQR))
            {
                Console.WriteLine("\n❌ Código vacío");
                Console.WriteLine("\nPresione cualquier tecla para continuar...");
                Console.ReadKey();
                return;
            }

            // Simular procesamiento
            await Task.Delay(500);

            Console.WriteLine();
            Console.WriteLine($"QR detectado: {codigoQR}");
            Console.WriteLine();

            if (_qrService.ValidarCodigoQR(codigoQR))
            {
                int id = _qrService.ExtraerIdDeQR(codigoQR);
                _envioActual = _repository.BuscarPorId(id);

                if (_envioActual != null)
                {
                    Console.WriteLine("✔ Envío identificado");
                    Console.WriteLine($"✔ Cliente: {_envioActual.Cliente}");
                    Console.WriteLine($"✔ Dirección: {_envioActual.Direccion} - {_envioActual.Ciudad}");
                    Console.WriteLine($"✔ Estado actual: {ObtenerTextoEstado(_envioActual.Estado)}");
                }
                else
                {
                    Console.WriteLine("❌ No se encontró el envío en el sistema");
                }
            }
            else
            {
                Console.WriteLine("❌ Código QR inválido");
            }

            Console.WriteLine("\nPresione cualquier tecla para continuar...");
            Console.ReadKey();
        }

        private static void VerDatosEnvio()
        {
            Console.Clear();

            if (_envioActual == null)
            {
                Console.WriteLine("❌ Primero debe escanear un código QR");
                Console.WriteLine("\nPresione cualquier tecla para continuar...");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("--- DATOS DE ENTREGA ---");
            Console.WriteLine($"Cliente: {_envioActual.Cliente}");
            Console.WriteLine($"Dirección: {_envioActual.Direccion} - {_envioActual.Ciudad}");
            Console.WriteLine($"Teléfono: {_envioActual.Telefono}");
            Console.WriteLine($"Peso: {_envioActual.Peso} kg");
            Console.WriteLine($"Costo: ${_envioActual.Costo:F2}");
            Console.WriteLine($"Estado: {ObtenerTextoEstado(_envioActual.Estado)}");

            Console.WriteLine("\nPresione cualquier tecla para continuar...");
            Console.ReadKey();
        }

        private static void TomarFotoEvidencia()
        {
            Console.Clear();

            if (_envioActual == null)
            {
                Console.WriteLine("❌ Primero debe escanear un código QR");
                Console.WriteLine("\nPresione cualquier tecla para continuar...");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("📸 Activando cámara...");
            Thread.Sleep(1500);
            Console.WriteLine();

            // Capturar evidencia
            string rutaEvidencia = _evidenciaService.CapturarEvidencia(_envioActual);

            if (!string.IsNullOrEmpty(rutaEvidencia))
            {
                Console.WriteLine("✔ Foto tomada correctamente");
                Console.WriteLine($"✔ Evidencia guardada en:");
                Console.WriteLine($"   {Path.GetFullPath(rutaEvidencia)}");

                // Actualizar en repositorio
                _repository.Actualizar(_envioActual);
            }
            else
            {
                Console.WriteLine("✗ Error al capturar evidencia");
            }

            Console.WriteLine("\nPresione cualquier tecla para continuar...");
            Console.ReadKey();
        }

        private static async Task ConfirmarEntrega()
        {
            Console.Clear();

            if (_envioActual == null)
            {
                Console.WriteLine("❌ Primero debe escanear un código QR");
                Console.WriteLine("\nPresione cualquier tecla para continuar...");
                Console.ReadKey();
                return;
            }

            if (string.IsNullOrEmpty(_envioActual.RutaEvidencia))
            {
                Console.WriteLine("❌ Debe tomar la foto de evidencia primero");
                Console.WriteLine("\nPresione cualquier tecla para continuar...");
                Console.ReadKey();
                return;
            }

            Console.Write($"¿Confirmar entrega del envío {_envioActual.Id}? (S/N): ");
            string? confirmacion = Console.ReadLine()?.ToUpper();

            if (confirmacion == "S")
            {
                Console.WriteLine();

                // Actualizar estado
                _envioActual.Estado = EstadoEnvio.Entregado;
                _envioActual.FechaEntrega = DateTime.Now;

                // Generar factura
                string rutaFactura = _pdfService.GenerarFactura(_envioActual);

                // Guardar cambios
                _repository.Actualizar(_envioActual);

                Console.WriteLine("✔ Estado actualizado: ENTREGADO");
                Console.WriteLine("✔ Evidencia fotográfica registrada");
                Console.WriteLine("✔ Factura PDF generada");

                if (!string.IsNullOrEmpty(rutaFactura))
                {
                    Console.WriteLine($"  Ruta: {Path.GetFullPath(rutaFactura)}");
                }

                // Enviar notificaciones
                try
                {
                    await _emailService.EnviarNotificacionEntrega(_envioActual);
                    Console.WriteLine("✔ Email enviado al cliente");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"✗ Error al enviar email: {ex.Message}");
                }

                // Nota: SMS comentado porque requiere Twilio real
                // await _smsService.EnviarSMSEntrega(_envioActual);
                // Console.WriteLine("✔ SMS enviado al cliente");

                Console.WriteLine();
                Console.WriteLine("✔ ENTREGA COMPLETADA CON ÉXITO");

                // Limpiar envío actual
                _envioActual = null;
            }
            else
            {
                Console.WriteLine("\n❌ Entrega cancelada");
            }

            Console.WriteLine("\nPresione cualquier tecla para continuar...");
            Console.ReadKey();
        }

        private static void EliminarEnvio()
        {
            Console.Clear();
            Console.Write("Ingrese el ID del envío a eliminar: ");

            if (int.TryParse(Console.ReadLine(), out int id))
            {
                var envio = _repository.BuscarPorId(id);

                if (envio != null)
                {
                    Console.WriteLine($"\n⚠️  Se eliminará el envío:");
                    Console.WriteLine($"   Cliente: {envio.Cliente}");
                    Console.WriteLine($"   Estado: {ObtenerTextoEstado(envio.Estado)}");
                    Console.Write("\n¿Está seguro? (S/N): ");

                    string? confirmacion = Console.ReadLine()?.ToUpper();

                    if (confirmacion == "S")
                    {
                        _repository.Eliminar(id);
                        Console.WriteLine("\n✔ Envío eliminado correctamente");
                    }
                    else
                    {
                        Console.WriteLine("\n❌ Operación cancelada");
                    }
                }
                else
                {
                    Console.WriteLine($"\n❌ No se encontró el envío con ID {id}");
                }
            }
            else
            {
                Console.WriteLine("\n❌ ID inválido");
            }
        }

        private static string ObtenerTextoEstado(EstadoEnvio estado)
        {
            return estado switch
            {
                EstadoEnvio.EnTransito => "En tránsito",
                EstadoEnvio.EnReparto => "En reparto",
                EstadoEnvio.Entregado => "Entregado",
                EstadoEnvio.Cancelado => "Cancelado",
                _ => "Desconocido"
            };
        }

        private static string TruncateString(string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return "";
            return value.Length <= maxLength ? value : value.Substring(0, maxLength - 3) + "...";
        }
    }
}