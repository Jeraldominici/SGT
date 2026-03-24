using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransporteEscolar.Repositories.Interfaces;
using TransporteEscolar.ViewModels;

namespace TransporteEscolar.Controllers
{
    [Authorize(Roles = "Padre")]
    public class PadreController : Controller
    {
        private readonly IUsuarioRepository _repo;

        public PadreController(IUsuarioRepository repo)
            => _repo = repo;

        private int GetUsuarioId() => int.Parse(
            User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);

        // ── Dashboard ────────────────────────────────────────
        public async Task<IActionResult> Dashboard()
        {
            var usuarioId = GetUsuarioId();
            var hijos = await _repo.ObtenerHijosDePadreAsync(usuarioId);

            var modelo = new PadreDashboardViewModel();

            foreach (var hijo in hijos)
            {
                var asistencias = await _repo.ObtenerAsistenciaAlumnoAsync(hijo.AlumnoId, 14);
                var incidencias = await _repo.ObtenerIncidenciasAlumnoAsync(hijo.AlumnoId);
                var jornada = await _repo.ObtenerJornadaActivaPorAutobusAsync(hijo.AutobusId);

                modelo.Hijos.Add(new HijoResumenViewModel
                {
                    Alumno = hijo,
                    Autobus = hijo.Autobus,
                    JornadaHoy = jornada,
                    Asistencias = asistencias,
                    Incidencias = incidencias
                });
            }

            ViewBag.NombreCompleto = User.FindFirst(
                System.Security.Claims.ClaimTypes.GivenName)?.Value;

            return View(modelo);
        }

        // ── Detalle de un hijo ───────────────────────────────
        public async Task<IActionResult> Hijo(int id)
        {
            var usuarioId = GetUsuarioId();
            var hijos = await _repo.ObtenerHijosDePadreAsync(usuarioId);
            var hijo = hijos.FirstOrDefault(h => h.AlumnoId == id);

            if (hijo is null) return Forbid();

            var asistencias = await _repo.ObtenerAsistenciaAlumnoAsync(hijo.AlumnoId, 30);
            var incidencias = await _repo.ObtenerIncidenciasAlumnoAsync(hijo.AlumnoId);
            var jornada = await _repo.ObtenerJornadaActivaPorAutobusAsync(hijo.AutobusId);

            var modelo = new HijoResumenViewModel
            {
                Alumno = hijo,
                Autobus = hijo.Autobus,
                JornadaHoy = jornada,
                Asistencias = asistencias,
                Incidencias = incidencias
            };

            return View(modelo);
        }
    }
}