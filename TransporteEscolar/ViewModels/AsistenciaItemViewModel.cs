namespace TransporteEscolar.ViewModels
{
    public class AsistenciaItemViewModel
    {
        public int AlumnoId { get; set; }
        public string NombreAlumno { get; set; } = string.Empty;
        public string GradoEscolar { get; set; } = string.Empty;
        public bool Presente { get; set; } = true;
        public string? Observacion { get; set; }
    }
}
