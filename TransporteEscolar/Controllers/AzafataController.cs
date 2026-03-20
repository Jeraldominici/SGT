using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransporteEscolar.Data;
using TransporteEscolar.Models;

namespace TransporteEscolar.Controllers
{
    [Authorize(Roles = "Azafata")]
    public class AzafataController : Controller
    {
        private readonly AppDbContext _context;

        public AzafataController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Dashboard()
        {
            ViewBag.NombreCompleto = User.FindFirst(System.Security.Claims.ClaimTypes.GivenName)?.Value;
            ViewBag.AutobusId = User.FindFirst("AutobusId")?.Value;
            return View();
        }

        [HttpPost]
        public IActionResult RegistrarQR([FromBody] string qr)
        {
            // Por ahora solo prueba
            Console.WriteLine("QR recibido: " + qr);

            return Ok(new { mensaje = "QR recibido correctamente" });
        }
    }
}