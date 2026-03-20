namespace TransporteEscolar.Models
{
    public class RegistroAbordaje
    {
        public int IdRegistro { get; set; }

        public int IdEstudiante { get; set; }
        public int FichaUnidad { get; set; }

        public DateTime Fecha { get; set; }

        public DateTime? HoraSubida { get; set; }
        public DateTime? HoraBajada { get; set; }

        public Estudiante Estudiante { get; set; } = null!;
    }
}