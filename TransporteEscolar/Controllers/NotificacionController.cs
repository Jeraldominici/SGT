using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransporteEscolar.Repositories.Interfaces;
using TransporteEscolar.Services;
using TransporteEscolar.ViewModels;

namespace TransporteEscolar.Controllers
{
    [Authorize]
    public class NotificacionController : Controller
    {
        private readonly IUsuarioRepository _repo;
        private readonly NotificacionService _notifService;

        public NotificacionController(IUsuarioRepository repo,
            NotificacionService notifService)
        {
            _repo = repo;
            _notifService = notifService;
        }

        private int GetUsuarioId() => int.Parse(
            User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);

        // ── Lista de notificaciones (Padre) ──────────────────
        [Authorize(Roles = "Padre")]
        public async Task<IActionResult> Index()
        {
            var uid = GetUsuarioId();
            var lista = await _repo.ObtenerNotificacionesAsync(uid);
            await _repo.MarcarTodasLeidasAsync(uid);
            return View(lista);
        }

        // ── Marcar una como leída (AJAX) ─────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarcarLeida(int id)
        {
            await _repo.MarcarLeidaAsync(id, GetUsuarioId());
            return Ok();
        }

        // ── Contador para el navbar (AJAX) ───────────────────
        [HttpGet]
        public async Task<IActionResult> Contador()
        {
            var count = await _repo.ContarNoLeidasAsync(GetUsuarioId());
            return Json(new { count });
        }

        // ── Enviar mensaje manual (Admin) ────────────────────
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> EnviarMensaje()
        {
            var usuarios = await _repo.ObtenerTodosAsync();
            ViewBag.Padres = usuarios.Where(u => u.Rol?.Nombre == "Padre").ToList();
            return View(new MensajeManualViewModel());
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnviarMensaje(MensajeManualViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                var usuarios = await _repo.ObtenerTodosAsync();
                ViewBag.Padres = usuarios.Where(u => u.Rol?.Nombre == "Padre").ToList();
                return View(modelo);
            }

            var destinatarios = modelo.EnviarATodos
                ? (await _repo.ObtenerTodosAsync())
                    .Where(u => u.Rol?.Nombre == "Padre")
                    .Select(u => u.UsuarioId).ToList()
                : modelo.UsuariosSeleccionados;

            await _notifService.NotificarManualAsync(
                destinatarios, modelo.Titulo, modelo.Mensaje);

            TempData["Exito"] = $"Mensaje enviado a {destinatarios.Count} padre(s).";
            return RedirectToAction(nameof(EnviarMensaje));
        }
    }
}