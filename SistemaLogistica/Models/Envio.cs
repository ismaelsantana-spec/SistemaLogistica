namespace SistemaLogistica.Models
{
    public enum EstadoEnvio
    {
        EnTransito,
        EnReparto,
        Entregado,
        Cancelado
    }

    public class Envio
    {
        public int Id { get; set; }
        public string Cliente { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string Ciudad { get; set; } = string.Empty;
        public double Peso { get; set; }
        public decimal Costo { get; set; }
        public EstadoEnvio Estado { get; set; }
        public DateTime FechaRegistro { get; set; }
        public DateTime? FechaEntrega { get; set; }

        // Logística y entrega
        public string CodigoQR { get; set; } = string.Empty;
        public string RutaQR { get; set; } = string.Empty;
        public string RutaEvidencia { get; set; } = string.Empty;
        public string RutaFactura { get; set; } = string.Empty;

        public Envio()
        {
            FechaRegistro = DateTime.Now;
            Estado = EstadoEnvio.EnTransito;
        }
    }
}