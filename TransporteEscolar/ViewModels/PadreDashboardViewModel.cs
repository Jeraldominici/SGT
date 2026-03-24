using TransporteEscolar.Models;

namespace TransporteEscolar.ViewModels
{
	public class PadreDashboardViewModel
	{
		public List<HijoResumenViewModel> Hijos { get; set; } = new();
	}

	public class HijoResumenViewModel
	{
		public Alumno Alumno { get; set; } = null!;
		public Autobus Autobus { get; set; } = null!;
		public JornadaChofer? JornadaHoy { get; set; }
		public List<Asistencia> Asistencias { get; set; } = new();
		public List<Incidencia> Incidencias { get; set; } = new();

		// Helpers
		public Asistencia? AsistenciaHoyIda => Asistencias
			.FirstOrDefault(a => a.Fecha == DateOnly.FromDateTime(DateTime.Today)
							  && a.TipoRuta == "Ida");
		public Asistencia? AsistenciaHoyVuelta => Asistencias
			.FirstOrDefault(a => a.Fecha == DateOnly.FromDateTime(DateTime.Today)
							  && a.TipoRuta == "Vuelta");
	}
}