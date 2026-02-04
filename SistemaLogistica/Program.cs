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
            ConfigurarConsola();
            InicializarServicios();
            await MostrarMenuPrincipal();
        }

        private static void ConfigurarConsola()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            try
            {
                Console.WindowWidth = 100;
                Console.WindowHeight = 30;
            }
            catch { /* En caso de que no se pueda ajustar */ }
        }

        private static void InicializarServicios()
        {
            MostrarPantallaCarga();
            
            _config = new Configuracion();
            _repository = new EnvioRepository();
            _qrService = new QRService();
            _pdfService = new PDFService(_config);
            _emailService = new EmailService(_config);
            _smsService = new SMSService(_config);
            _evidenciaService = new EvidenciaService();

            Thread.Sleep(1500);
        }

        private static void MostrarPantallaCarga()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            
            Console.WriteLine("\n\n\n");
            Console.WriteLine("    ╔══════════════════════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("    ║                                                                                  ║");
            Console.WriteLine("    ║      ███████╗██╗  ██╗██████╗ ██████╗ ███████╗███████╗███████╗                   ║");
            Console.WriteLine("    ║      ██╔════╝╚██╗██╔╝██╔══██╗██╔══██╗██╔════╝██╔════╝██╔════╝                   ║");
            Console.WriteLine("    ║      █████╗   ╚███╔╝ ██████╔╝██████╔╝█████╗  ███████╗███████╗                   ║");
            Console.WriteLine("    ║      ██╔══╝   ██╔██╗ ██╔═══╝ ██╔══██╗██╔══╝  ╚════██║╚════██║                   ║");
            Console.WriteLine("    ║      ███████╗██╔╝ ██╗██║     ██║  ██║███████╗███████║███████║                   ║");
            Console.WriteLine("    ║      ╚══════╝╚═╝  ╚═╝╚═╝     ╚═╝  ╚═╝╚══════╝╚══════╝╚══════╝                   ║");
            Console.WriteLine("    ║                                                                                  ║");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("    ║                    Sistema de Logística y Entrega de Paquetería                 ║");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("    ║                                  v1.0.0                                          ║");
            Console.WriteLine("    ║                                                                                  ║");
            Console.WriteLine("    ╚══════════════════════════════════════════════════════════════════════════════════╝");
            Console.ResetColor();
            
            Console.WriteLine("\n");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("    ⚙️  Inicializando sistema");
            for (int i = 0; i < 5; i++)
            {
                Thread.Sleep(300);
                Console.Write(".");
            }
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"    ✓ Directorio de trabajo: {Directory.GetCurrentDirectory()}");
            Console.WriteLine("    ✓ Servicios cargados correctamente");
            Console.ResetColor();
        }

        private static async Task MostrarMenuPrincipal()
        {
            bool salir = false;

            while (!salir)
            {
                Console.Clear();
                
                // Header
                Console.ForegroundColor = ConsoleColor.White;
                Console.BackgroundColor = ConsoleColor.DarkBlue;
                Console.WriteLine("╔════════════════════════════════════════════════════════════════════════════════════════════════╗");
                Console.WriteLine("║                     🚚 SISTEMA DE LOGÍSTICA Y ENTREGA DE PAQUETERÍA 📦                         ║");
                Console.WriteLine("╚════════════════════════════════════════════════════════════════════════════════════════════════╝");
                Console.ResetColor();
                
                Console.WriteLine();
                
                // Módulo Logística
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("  ┌─────────────────────────────────────────────┐");
                Console.WriteLine("  │      📋 MÓDULO DE LOGÍSTICA                 │");
                Console.WriteLine("  └─────────────────────────────────────────────┘");
                Console.ResetColor();
                
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("    [1] 📝 Registrar nuevo envío");
                Console.WriteLine("    [2] 📊 Listar envíos y estados");
                Console.WriteLine("    [3] 🔍 Buscar envío específico");
                Console.ResetColor();
                
                Console.WriteLine();
                
                // Módulo Repartidor
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("  ┌─────────────────────────────────────────────┐");
                Console.WriteLine("  │      🚛 MÓDULO DE REPARTIDOR                │");
                Console.WriteLine("  └─────────────────────────────────────────────┘");
                Console.ResetColor();
                
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("    [4] 📲 Gestión de entrega (escaneo QR)");
                Console.ResetColor();
                
                Console.WriteLine();
                
                // Administración
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("  ┌─────────────────────────────────────────────┐");
                Console.WriteLine("  │      ⚙️  ADMINISTRACIÓN                     │");
                Console.WriteLine("  └─────────────────────────────────────────────┘");
                Console.ResetColor();
                
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("    [5] 🗑️  Eliminar envío");
                Console.WriteLine("    [6] 🚪 Salir del sistema");
                Console.ResetColor();
                
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("  ════════════════════════════════════════════════════════════════════════════════");
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("  ➤ Seleccione una opción: ");
                Console.ForegroundColor = ConsoleColor.Yellow;

                string? opcion = Console.ReadLine();
                Console.ResetColor();

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
                        MostrarDespedida();
                        break;
                    default:
                        MostrarError("Opción inválida. Intente nuevamente.");
                        break;
                }

                if (!salir && opcion != "6")
                {
                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write("  Presione cualquier tecla para continuar...");
                    Console.ResetColor();
                    Console.ReadKey();
                }
            }
        }

        private static async Task RegistrarEnvio()
        {
            Console.Clear();
            DibujarEncabezado("📝 REGISTRO DE NUEVO ENVÍO", ConsoleColor.Cyan);
            
            var envio = new Envio
            {
                Id = _repository.ObtenerSiguienteId()
            };

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"  📋 ID de Envío: {envio.Id:D6}");
            Console.ResetColor();
            Console.WriteLine();

            envio.Cliente = SolicitarDato("👤 Nombre del cliente", true);
            envio.Telefono = SolicitarDato("📞 Teléfono de contacto", true);
            envio.Email = SolicitarDato("📧 Correo electrónico", true);
            envio.Direccion = SolicitarDato("📍 Dirección de entrega", true);
            envio.Ciudad = SolicitarDato("🏙️  Ciudad", true);
            
            Console.Write("  ⚖️  Peso (kg): ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            if (double.TryParse(Console.ReadLine(), out double peso))
            {
                envio.Peso = peso;
            }
            Console.ResetColor();

            Console.Write("  💰 Costo ($): ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            if (decimal.TryParse(Console.ReadLine(), out decimal costo))
            {
                envio.Costo = costo;
            }
            Console.ResetColor();

            Console.WriteLine();
            DibujarSeparador();
            Console.WriteLine();

            // Generar QR
            MostrarProceso("Generando código QR");
            string rutaQR = _qrService.GenerarCodigoQR(envio);

            if (!string.IsNullOrEmpty(rutaQR))
            {
                MostrarExito($"Código QR generado: {envio.CodigoQR}");
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine($"  └─ Archivo: {Path.GetFullPath(rutaQR)}");
                Console.ResetColor();
            }
            else
            {
                MostrarError("Error al generar código QR");
            }

            // Generar factura
            MostrarProceso("Generando factura PDF");
            string rutaFactura = _pdfService.GenerarFactura(envio);
            MostrarExito("Factura PDF generada");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"  └─ Archivo: {Path.GetFullPath(rutaFactura)}");
            Console.ResetColor();

            // Guardar en repositorio
            _repository.Agregar(envio);

            // Enviar email con factura
            if (!string.IsNullOrEmpty(envio.Email))
            {
                Console.WriteLine();
                MostrarProceso("Enviando factura por correo electrónico");

                try
                {
                    bool emailEnviado = await _emailService.EnviarFacturaPorEmail(envio);

                    if (emailEnviado)
                    {
                        MostrarExito("Correo electrónico enviado correctamente");
                    }
                    else
                    {
                        MostrarAdvertencia("No se pudo enviar el email (verifica la configuración)");
                    }
                }
                catch (Exception ex)
                {
                    MostrarError($"Error al enviar email: {ex.Message}");
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.WriteLine("  💡 Configura tu email en Models/Configuracion.cs");
                    Console.ResetColor();
                }
            }

            Console.WriteLine();
            DibujarSeparador();
            Console.WriteLine();
            
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("  ╔════════════════════════════════════════════╗");
            Console.WriteLine("  ║  ✅ ENVÍO REGISTRADO EXITOSAMENTE          ║");
            Console.WriteLine("  ╚════════════════════════════════════════════╝");
            Console.ResetColor();
            
            Console.WriteLine();
            MostrarExito("Estado inicial: EN TRÁNSITO");
            MostrarExito("Paquete listo para despacho");
        }

        private static void ListarEnvios()
        {
            Console.Clear();
            DibujarEncabezado("📊 CONTROL Y SEGUIMIENTO DE ENVÍOS", ConsoleColor.Cyan);

            var envios = _repository.ObtenerTodos();

            if (envios.Count == 0)
            {
                MostrarAdvertencia("No hay envíos registrados en el sistema");
                return;
            }

            Console.WriteLine();
            
            // Contador por estados
            var enTransito = envios.Count(e => e.Estado == EstadoEnvio.EnTransito);
            var enReparto = envios.Count(e => e.Estado == EstadoEnvio.EnReparto);
            var entregados = envios.Count(e => e.Estado == EstadoEnvio.Entregado);
            var cancelados = envios.Count(e => e.Estado == EstadoEnvio.Cancelado);
            
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("  📈 ESTADÍSTICAS:");
            Console.ResetColor();
            Console.WriteLine($"     🔵 En tránsito: {enTransito}  |  🟡 En reparto: {enReparto}  |  ✅ Entregados: {entregados}  |  ❌ Cancelados: {cancelados}");
            Console.WriteLine();
            DibujarSeparador();
            Console.WriteLine();

            // Tabla de envíos
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("  ┌──────┬────────────────────┬─────────────────┬──────────────┬────────────┐");
            Console.WriteLine("  │  ID  │      CLIENTE       │     CIUDAD      │    ESTADO    │    COSTO   │");
            Console.WriteLine("  ├──────┼────────────────────┼─────────────────┼──────────────┼────────────┤");
            Console.ResetColor();

            foreach (var envio in envios)
            {
                Console.Write("  │ ");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write($"{envio.Id,-4}");
                Console.ResetColor();
                
                Console.Write(" │ ");
                Console.Write($"{TruncateString(envio.Cliente, 18),-18}");
                
                Console.Write(" │ ");
                Console.Write($"{TruncateString(envio.Ciudad, 15),-15}");
                
                Console.Write(" │ ");
                
                // Color según estado
                switch (envio.Estado)
                {
                    case EstadoEnvio.EnTransito:
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.Write("🔵 Tránsito ");
                        break;
                    case EstadoEnvio.EnReparto:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write("🟡 Reparto  ");
                        break;
                    case EstadoEnvio.Entregado:
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write("✅ Entregado");
                        break;
                    case EstadoEnvio.Cancelado:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("❌ Cancelado");
                        break;
                }
                Console.ResetColor();
                
                Console.Write(" │ ");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write($"${envio.Costo,8:F2}");
                Console.ResetColor();
                Console.WriteLine(" │");
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("  └──────┴────────────────────┴─────────────────┴──────────────┴────────────┘");
            Console.ResetColor();
            
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"  📦 Total de envíos registrados: {envios.Count}");
            Console.ResetColor();
        }

        private static void BuscarEnvio()
        {
            Console.Clear();
            DibujarEncabezado("🔍 BÚSQUEDA Y SEGUIMIENTO DE ENVÍO", ConsoleColor.Magenta);

            Console.Write("  🔎 Ingrese el ID del envío: ");
            Console.ForegroundColor = ConsoleColor.Yellow;

            if (int.TryParse(Console.ReadLine(), out int id))
            {
                Console.ResetColor();
                MostrarProceso("Buscando envío en la base de datos");
                Thread.Sleep(500);
                
                var envio = _repository.BuscarPorId(id);

                if (envio != null)
                {
                    Console.WriteLine();
                    DibujarSeparador();
                    Console.WriteLine();
                    
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("  ╔════════════════════════════════════════════╗");
                    Console.WriteLine("  ║       📋 INFORMACIÓN DEL ENVÍO             ║");
                    Console.WriteLine("  ╚════════════════════════════════════════════╝");
                    Console.ResetColor();
                    Console.WriteLine();
                    
                    Console.WriteLine($"  🆔 ID: {envio.Id:D6}");
                    Console.WriteLine($"  👤 Cliente: {envio.Cliente}");
                    Console.WriteLine($"  📞 Teléfono: {envio.Telefono}");
                    Console.WriteLine($"  📧 Email: {envio.Email}");
                    Console.WriteLine($"  📍 Destino: {envio.Direccion}");
                    Console.WriteLine($"  🏙️  Ciudad: {envio.Ciudad}");
                    Console.WriteLine($"  ⚖️  Peso: {envio.Peso} kg");
                    Console.WriteLine($"  💰 Costo: ${envio.Costo:F2}");
                    Console.WriteLine();
                    
                    Console.Write("  📊 Estado: ");
                    switch (envio.Estado)
                    {
                        case EstadoEnvio.EnTransito:
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.WriteLine("🔵 EN TRÁNSITO");
                            break;
                        case EstadoEnvio.EnReparto:
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine("🟡 EN REPARTO");
                            break;
                        case EstadoEnvio.Entregado:
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("✅ ENTREGADO");
                            break;
                        case EstadoEnvio.Cancelado:
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("❌ CANCELADO");
                            break;
                    }
                    Console.ResetColor();
                    
                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("  ┌─────────────────────────────────────────┐");
                    Console.WriteLine("  │  📲 INFORMACIÓN TÉCNICA                 │");
                    Console.WriteLine("  └─────────────────────────────────────────┘");
                    Console.ResetColor();
                    
                    Console.Write("  🔲 Código QR: ");
                    if (!string.IsNullOrEmpty(envio.CodigoQR))
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"✓ {envio.CodigoQR}");
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("✗ No generado");
                    }
                    Console.ResetColor();
                    
                    Console.Write("  📸 Evidencia: ");
                    if (!string.IsNullOrEmpty(envio.RutaEvidencia))
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("✓ Capturada");
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("⏳ Pendiente");
                    }
                    Console.ResetColor();

                    if (!string.IsNullOrEmpty(envio.RutaQR))
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.WriteLine($"  └─ Ruta QR: {Path.GetFullPath(envio.RutaQR)}");
                        Console.ResetColor();
                    }
                }
                else
                {
                    Console.WriteLine();
                    MostrarError($"No se encontró ningún envío con el ID: {id:D6}");
                }
            }
            else
            {
                Console.ResetColor();
                MostrarError("ID inválido. Debe ingresar un número");
            }
        }

        private static async Task ModuloEntrega()
        {
            bool volverMenu = false;

            while (!volverMenu)
            {
                Console.Clear();
                
                Console.ForegroundColor = ConsoleColor.White;
                Console.BackgroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("╔════════════════════════════════════════════════════════════════════════════════════════════════╗");
                Console.WriteLine("║                              🚛 MÓDULO DE ENTREGA - REPARTIDOR 📦                             ║");
                Console.WriteLine("╚════════════════════════════════════════════════════════════════════════════════════════════════╝");
                Console.ResetColor();
                
                Console.WriteLine();
                
                if (_envioActual != null)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("  ╔═══════════════════════════════════════════════════════════════════╗");
                    Console.WriteLine($"  ║  ✅ ENVÍO CARGADO: {_envioActual.Id:D6} - {TruncateString(_envioActual.Cliente, 35),-35} ║");
                    Console.WriteLine("  ╚═══════════════════════════════════════════════════════════════════╝");
                    Console.ResetColor();
                    Console.WriteLine();
                }
                
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("  ┌─────────────────────────────────────────────┐");
                Console.WriteLine("  │      🔧 OPCIONES DISPONIBLES                │");
                Console.WriteLine("  └─────────────────────────────────────────────┘");
                Console.ResetColor();
                
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("    [1] 📲 Escanear código QR del paquete");
                Console.WriteLine("    [2] 📋 Ver datos completos del envío");
                Console.WriteLine("    [3] 📸 Tomar foto de evidencia");
                Console.WriteLine("    [4] ✅ Confirmar entrega completada");
                Console.WriteLine("    [5] ⬅️  Volver al menú principal");
                Console.ResetColor();
                
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("  ════════════════════════════════════════════════════════════════════════════════");
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("  ➤ Seleccione una opción: ");
                Console.ForegroundColor = ConsoleColor.Yellow;

                string? opcion = Console.ReadLine();
                Console.ResetColor();

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
                        MostrarError("Opción inválida");
                        Console.WriteLine();
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write("  Presione cualquier tecla para continuar...");
                        Console.ResetColor();
                        Console.ReadKey();
                        break;
                }
            }
        }

        private static Envio? _envioActual = null;

        private static async Task EscanearQR()
        {
            Console.Clear();
            DibujarEncabezado("📲 ESCANEO DE CÓDIGO QR", ConsoleColor.Yellow);

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("  ┌─────────────────────────────────────────────┐");
            Console.WriteLine("  │      🔍 OPCIONES DE ESCANEO                 │");
            Console.WriteLine("  └─────────────────────────────────────────────┘");
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine("    [1] 📸 Escanear con cámara web (REAL)");
            Console.WriteLine("    [2] ⌨️  Ingresar código manualmente");
            Console.WriteLine();
            Console.Write("  ➤ Seleccione opción: ");
            Console.ForegroundColor = ConsoleColor.Yellow;

            string? opcion = Console.ReadLine();
            Console.ResetColor();
            string? codigoQR = null;

            if (opcion == "1")
            {
                Console.WriteLine();
                MostrarProceso("Abriendo cámara web");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("  ⏳ Acerque el código QR a la cámara...");
                Console.ResetColor();

                await Task.Delay(1000);

                // Escanear con cámara REAL
                codigoQR = _qrService.EscanearQRConCamara();

                if (string.IsNullOrEmpty(codigoQR))
                {
                    MostrarError("Escaneo cancelado o sin resultado");
                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write("  Presione cualquier tecla para continuar...");
                    Console.ResetColor();
                    Console.ReadKey();
                    return;
                }
            }
            else if (opcion == "2")
            {
                Console.WriteLine();
                Console.Write("  🔲 Ingrese el código QR: ");
                Console.ForegroundColor = ConsoleColor.Yellow;
                codigoQR = Console.ReadLine();
                Console.ResetColor();
            }
            else
            {
                MostrarError("Opción inválida");
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write("  Presione cualquier tecla para continuar...");
                Console.ResetColor();
                Console.ReadKey();
                return;
            }

            if (string.IsNullOrEmpty(codigoQR))
            {
                MostrarError("Código vacío. Intente nuevamente");
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write("  Presione cualquier tecla para continuar...");
                Console.ResetColor();
                Console.ReadKey();
                return;
            }

            // Simular procesamiento
            MostrarProceso("Validando código QR");
            await Task.Delay(500);

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"  🔲 QR detectado: {codigoQR}");
            Console.ResetColor();
            Console.WriteLine();

            if (_qrService.ValidarCodigoQR(codigoQR))
            {
                int id = _qrService.ExtraerIdDeQR(codigoQR);
                _envioActual = _repository.BuscarPorId(id);

                if (_envioActual != null)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("  ╔════════════════════════════════════════════╗");
                    Console.WriteLine("  ║  ✅ ENVÍO IDENTIFICADO CORRECTAMENTE       ║");
                    Console.WriteLine("  ╚════════════════════════════════════════════╝");
                    Console.ResetColor();
                    Console.WriteLine();
                    MostrarExito($"ID: {_envioActual.Id:D6}");
                    MostrarExito($"Cliente: {_envioActual.Cliente}");
                    MostrarExito($"Dirección: {_envioActual.Direccion} - {_envioActual.Ciudad}");
                    Console.Write("  ✅ Estado actual: ");
                    
                    switch (_envioActual.Estado)
                    {
                        case EstadoEnvio.EnTransito:
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.WriteLine("🔵 EN TRÁNSITO");
                            break;
                        case EstadoEnvio.EnReparto:
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine("🟡 EN REPARTO");
                            break;
                        case EstadoEnvio.Entregado:
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("✅ ENTREGADO");
                            break;
                    }
                    Console.ResetColor();
                }
                else
                {
                    MostrarError("No se encontró el envío en el sistema");
                }
            }
            else
            {
                MostrarError("Código QR inválido o no reconocido");
            }

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("  Presione cualquier tecla para continuar...");
            Console.ResetColor();
            Console.ReadKey();
        }

        private static void VerDatosEnvio()
        {
            Console.Clear();

            if (_envioActual == null)
            {
                DibujarEncabezado("❌ ERROR - SIN ENVÍO CARGADO", ConsoleColor.Red);
                MostrarError("Primero debe escanear un código QR");
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write("  Presione cualquier tecla para continuar...");
                Console.ResetColor();
                Console.ReadKey();
                return;
            }

            DibujarEncabezado($"📋 DATOS DEL ENVÍO {_envioActual.Id:D6}", ConsoleColor.Cyan);
            
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("  ┌──────────────────────────────────────────────┐");
            Console.WriteLine("  │      📦 INFORMACIÓN DE ENTREGA               │");
            Console.WriteLine("  └──────────────────────────────────────────────┘");
            Console.ResetColor();
            Console.WriteLine();
            
            Console.WriteLine($"  👤 Cliente: {_envioActual.Cliente}");
            Console.WriteLine($"  📍 Dirección: {_envioActual.Direccion}");
            Console.WriteLine($"  🏙️  Ciudad: {_envioActual.Ciudad}");
            Console.WriteLine($"  📞 Teléfono: {_envioActual.Telefono}");
            Console.WriteLine($"  📧 Email: {_envioActual.Email}");
            Console.WriteLine($"  ⚖️  Peso: {_envioActual.Peso} kg");
            Console.WriteLine($"  💰 Costo: ${_envioActual.Costo:F2}");
            Console.WriteLine();
            
            Console.Write("  📊 Estado: ");
            switch (_envioActual.Estado)
            {
                case EstadoEnvio.EnTransito:
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine("🔵 EN TRÁNSITO");
                    break;
                case EstadoEnvio.EnReparto:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("🟡 EN REPARTO");
                    break;
                case EstadoEnvio.Entregado:
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("✅ ENTREGADO");
                    break;
            }
            Console.ResetColor();

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("  Presione cualquier tecla para continuar...");
            Console.ResetColor();
            Console.ReadKey();
        }

        private static void TomarFotoEvidencia()
        {
            Console.Clear();

            if (_envioActual == null)
            {
                DibujarEncabezado("❌ ERROR - SIN ENVÍO CARGADO", ConsoleColor.Red);
                MostrarError("Primero debe escanear un código QR");
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write("  Presione cualquier tecla para continuar...");
                Console.ResetColor();
                Console.ReadKey();
                return;
            }

            DibujarEncabezado("📸 CAPTURA DE EVIDENCIA FOTOGRÁFICA", ConsoleColor.Green);

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("  ⏳ Activando cámara del dispositivo...");
            Console.ResetColor();
            Thread.Sleep(800);
            
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("  📷 Preparando captura...");
            Console.ResetColor();
            Thread.Sleep(700);
            
            Console.WriteLine();
            MostrarProceso("Capturando evidencia fotográfica");

            // Capturar evidencia
            string rutaEvidencia = _evidenciaService.CapturarEvidencia(_envioActual);

            if (!string.IsNullOrEmpty(rutaEvidencia))
            {
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("  ╔════════════════════════════════════════════╗");
                Console.WriteLine("  ║  ✅ EVIDENCIA CAPTURADA EXITOSAMENTE       ║");
                Console.WriteLine("  ╚════════════════════════════════════════════╝");
                Console.ResetColor();
                Console.WriteLine();
                MostrarExito("Foto guardada correctamente");
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine($"  📁 Ubicación: {Path.GetFullPath(rutaEvidencia)}");
                Console.ResetColor();

                // Actualizar en repositorio
                _repository.Actualizar(_envioActual);
            }
            else
            {
                MostrarError("Error al capturar evidencia. Intente nuevamente");
            }

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("  Presione cualquier tecla para continuar...");
            Console.ResetColor();
            Console.ReadKey();
        }

        private static async Task ConfirmarEntrega()
        {
            Console.Clear();

            if (_envioActual == null)
            {
                DibujarEncabezado("❌ ERROR - SIN ENVÍO CARGADO", ConsoleColor.Red);
                MostrarError("Primero debe escanear un código QR");
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write("  Presione cualquier tecla para continuar...");
                Console.ResetColor();
                Console.ReadKey();
                return;
            }

            if (string.IsNullOrEmpty(_envioActual.RutaEvidencia))
            {
                DibujarEncabezado("⚠️  ADVERTENCIA - FALTA EVIDENCIA", ConsoleColor.Yellow);
                MostrarAdvertencia("Debe tomar la foto de evidencia antes de confirmar la entrega");
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write("  Presione cualquier tecla para continuar...");
                Console.ResetColor();
                Console.ReadKey();
                return;
            }

            DibujarEncabezado("✅ CONFIRMACIÓN DE ENTREGA", ConsoleColor.Green);
            
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("  ┌──────────────────────────────────────────────┐");
            Console.WriteLine("  │      📦 RESUMEN DE ENTREGA                   │");
            Console.WriteLine("  └──────────────────────────────────────────────┘");
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine($"  🆔 ID Envío: {_envioActual.Id:D6}");
            Console.WriteLine($"  👤 Cliente: {_envioActual.Cliente}");
            Console.WriteLine($"  📍 Dirección: {_envioActual.Direccion}, {_envioActual.Ciudad}");
            Console.WriteLine($"  📸 Evidencia: ✅ Capturada");
            Console.WriteLine();
            DibujarSeparador();
            Console.WriteLine();
            
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("  ⚠️  ¿Confirmar entrega del envío? (S/N): ");
            Console.ForegroundColor = ConsoleColor.White;
            string? confirmacion = Console.ReadLine()?.ToUpper();
            Console.ResetColor();

            if (confirmacion == "S")
            {
                Console.WriteLine();
                MostrarProceso("Procesando confirmación de entrega");
                await Task.Delay(800);

                // Actualizar estado
                _envioActual.Estado = EstadoEnvio.Entregado;
                _envioActual.FechaEntrega = DateTime.Now;

                // Generar factura
                MostrarProceso("Generando factura final");
                string rutaFactura = _pdfService.GenerarFactura(_envioActual);

                // Guardar cambios
                _repository.Actualizar(_envioActual);

                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("  ╔════════════════════════════════════════════╗");
                Console.WriteLine("  ║  ✅ ENTREGA COMPLETADA EXITOSAMENTE        ║");
                Console.WriteLine("  ╚════════════════════════════════════════════╝");
                Console.ResetColor();
                Console.WriteLine();

                MostrarExito("Estado actualizado: ENTREGADO");
                MostrarExito("Evidencia fotográfica registrada");
                MostrarExito("Factura PDF generada");

                if (!string.IsNullOrEmpty(rutaFactura))
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.WriteLine($"  └─ {Path.GetFullPath(rutaFactura)}");
                    Console.ResetColor();
                }

                // Enviar notificaciones
                Console.WriteLine();
                MostrarProceso("Enviando notificación al cliente");
                
                try
                {
                    await _emailService.EnviarNotificacionEntrega(_envioActual);
                    MostrarExito("Correo electrónico enviado al cliente");
                }
                catch (Exception ex)
                {
                    MostrarError($"Error al enviar email: {ex.Message}");
                }

                Console.WriteLine();
                DibujarSeparador();
                Console.WriteLine();
                
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("  🎉 PROCESO DE ENTREGA FINALIZADO");
                Console.ResetColor();

                // Limpiar envío actual
                _envioActual = null;
            }
            else
            {
                Console.WriteLine();
                MostrarAdvertencia("Entrega cancelada por el usuario");
            }

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("  Presione cualquier tecla para continuar...");
            Console.ResetColor();
            Console.ReadKey();
        }

        private static void EliminarEnvio()
        {
            Console.Clear();
            DibujarEncabezado("🗑️  ELIMINACIÓN DE ENVÍO", ConsoleColor.Red);

            Console.Write("  🔎 Ingrese el ID del envío a eliminar: ");
            Console.ForegroundColor = ConsoleColor.Yellow;

            if (int.TryParse(Console.ReadLine(), out int id))
            {
                Console.ResetColor();
                
                var envio = _repository.BuscarPorId(id);

                if (envio != null)
                {
                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("  ⚠️  ADVERTENCIA: Se eliminará el siguiente envío:");
                    Console.ResetColor();
                    Console.WriteLine();
                    Console.WriteLine($"  🆔 ID: {envio.Id:D6}");
                    Console.WriteLine($"  👤 Cliente: {envio.Cliente}");
                    Console.WriteLine($"  📍 Destino: {envio.Ciudad}");
                    Console.Write($"  📊 Estado: ");
                    
                    switch (envio.Estado)
                    {
                        case EstadoEnvio.EnTransito:
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.WriteLine("🔵 EN TRÁNSITO");
                            break;
                        case EstadoEnvio.EnReparto:
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine("🟡 EN REPARTO");
                            break;
                        case EstadoEnvio.Entregado:
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("✅ ENTREGADO");
                            break;
                    }
                    Console.ResetColor();
                    
                    Console.WriteLine();
                    DibujarSeparador();
                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("  ⚠️  ¿Está seguro que desea eliminar este envío? (S/N): ");
                    Console.ForegroundColor = ConsoleColor.White;

                    string? confirmacion = Console.ReadLine()?.ToUpper();
                    Console.ResetColor();

                    if (confirmacion == "S")
                    {
                        _repository.Eliminar(id);
                        Console.WriteLine();
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("  ╔════════════════════════════════════════════╗");
                        Console.WriteLine("  ║  ✅ ENVÍO ELIMINADO CORRECTAMENTE          ║");
                        Console.WriteLine("  ╚════════════════════════════════════════════╝");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.WriteLine();
                        MostrarAdvertencia("Operación de eliminación cancelada");
                    }
                }
                else
                {
                    Console.ResetColor();
                    Console.WriteLine();
                    MostrarError($"No se encontró el envío con ID: {id:D6}");
                }
            }
            else
            {
                Console.ResetColor();
                MostrarError("ID inválido. Debe ingresar un número");
            }
        }

        // ============================================
        // MÉTODOS AUXILIARES DE DISEÑO
        // ============================================

        private static void DibujarEncabezado(string titulo, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine("  ╔════════════════════════════════════════════════════════════════════════════════╗");
            Console.WriteLine($"  ║  {titulo.PadRight(78)}║");
            Console.WriteLine("  ╚════════════════════════════════════════════════════════════════════════════════╝");
            Console.ResetColor();
            Console.WriteLine();
        }

        private static void DibujarSeparador()
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("  ─────────────────────────────────────────────────────────────────────────────────");
            Console.ResetColor();
        }

        private static void MostrarExito(string mensaje)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"  ✅ {mensaje}");
            Console.ResetColor();
        }

        private static void MostrarError(string mensaje)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"  ❌ {mensaje}");
            Console.ResetColor();
        }

        private static void MostrarAdvertencia(string mensaje)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"  ⚠️  {mensaje}");
            Console.ResetColor();
        }

        private static void MostrarProceso(string mensaje)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"  ⏳ {mensaje}");
            for (int i = 0; i < 3; i++)
            {
                Thread.Sleep(300);
                Console.Write(".");
            }
            Console.WriteLine();
            Console.ResetColor();
        }

        private static string SolicitarDato(string campo, bool obligatorio = false)
        {
            Console.Write($"  {campo}: ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            string? valor = Console.ReadLine() ?? "";
            Console.ResetColor();
            return valor;
        }

        private static void MostrarDespedida()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n\n");
            Console.WriteLine("  ╔════════════════════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("  ║                                                                                ║");
            Console.WriteLine("  ║                    👋 ¡GRACIAS POR USAR NUESTRO SISTEMA!                       ║");
            Console.WriteLine("  ║                                                                                ║");
            Console.WriteLine("  ║                         Sistema de Logística y Paquetería                     ║");
            Console.WriteLine("  ║                                   v1.0.0                                       ║");
            Console.WriteLine("  ║                                                                                ║");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("  ║                    🚚 Entregando sonrisas, un paquete a la vez 📦              ║");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("  ║                                                                                ║");
            Console.WriteLine("  ╚════════════════════════════════════════════════════════════════════════════════╝");
            Console.ResetColor();
            Console.WriteLine("\n\n");
            Thread.Sleep(2000);
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
