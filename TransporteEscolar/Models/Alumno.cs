namespace TransporteEscolar.Models
{
    public class Alumno
    {
        public int AlumnoId { get; set; }

        public string NombreCompleto { get; set; } = "";

        public DateTime? FechaNacimiento { get; set; }

        public string? GradoEscolar { get; set; }

        public string? NombreTutor { get; set; }

        public string? TelefonoEmergencia { get; set; }

        public string? FotoUrl { get; set; }

        public string? DireccionRecogida { get; set; }

        public string? DireccionEntrega { get; set; }

        public int AutobusId { get; set; }

        public bool Activo { get; set; }

        public DateTime FechaAlta { get; set; }

        // Relación
        public Autobus Autobus { get; set; } = null!;
    }
}