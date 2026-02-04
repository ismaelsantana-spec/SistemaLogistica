namespace SistemaLogistica.Models
{
    public class Configuracion
    {
        // Configuración de Email (Gmail)
        public string SmtpHost { get; set; } = "smtp.gmail.com";
        public int SmtpPort { get; set; } = 587;
        public string EmailFrom { get; set; } = "as3149481@gmail.com";
        public string EmailPassword { get; set; } = "ccus ycqx kaco ezhm";
        // Configuración de Twilio para SMS
        public string TwilioAccountSid { get; set; } = "TU_ACCOUNT_SID";
        public string TwilioAuthToken { get; set; } = "TU_AUTH_TOKEN";
        public string TwilioPhoneNumber { get; set; } = "+1234567890";

        // Información de la empresa
        public string NombreEmpresa { get; set; } = "Logística Express";
        public string RucEmpresa { get; set; } = "0999999999001";
        public string DireccionEmpresa { get; set; } = "Av. Principal 123";
        public string TelefonoEmpresa { get; set; } = "02-2999999";
    }
}