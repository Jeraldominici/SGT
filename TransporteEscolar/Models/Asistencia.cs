namespace TransporteEscolar.Models
{
    public class Asistencia
    {
        public int AsistenciaId { get; set; }

        public int AlumnoId { get; set; }
        public int AutobusId { get; set; }
        public int UsuarioId { get; set; }

        public DateTime Fecha { get; set; }
        public string TipoRuta { get; set; } = "";
        public bool Presente { get; set; }
        public DateTime FechaHora { get; set; }

        //  ESTO
        public Alumno Alumno { get; set; } = null!;
    }
}