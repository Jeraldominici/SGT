namespace TransporteEscolar.Models
{
    public class Estudiante
    {
        public int IdEstudiante { get; set; }

        public string Nombre { get; set; } = string.Empty;
        public string Apellido1 { get; set; } = string.Empty;
        public string Apellido2 { get; set; } = string.Empty;

        public int IdRuta { get; set; }

        public string Direccion { get; set; } = string.Empty;
        public string NombrePadre { get; set; } = string.Empty;
        public string TelefonoPadre { get; set; } = string.Empty;

        public int Grado { get; set; }

        // Código QR (IMPORTANTE)
        public string CodigoQR { get; set; } = string.Empty;
    }
}