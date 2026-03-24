using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QRCoder;
using System.Security.Claims;
using TransporteEscolar.Data;
using TransporteEscolar.Models;
using QRCoder;
using System.Drawing;
using System.IO;


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

        // 🚌 Dashboard
        public IActionResult Dashboard()
        {
            ViewBag.AutobusId = User.FindFirst("AutobusId")?.Value;
            return View();
        }

        // 👶 Lista de alumnos
        public IActionResult ListaAlumnos()
        {
            var autobusId = int.Parse(User.FindFirst("AutobusId")?.Value ?? "0");
            var hoy = DateTime.Now.Date;

            var alumnos = _context.Alumnos
                .Where(a => a.AutobusId == autobusId && a.Activo)
                .ToList();

            var asistenciasHoy = _context.Asistencias
                .Where(a => a.AutobusId == autobusId && a.Fecha == hoy)
                .ToList();

            var resultado = alumnos.Select(a => new
            {
                Alumno = a,
                Estado = asistenciasHoy
                    .Where(x => x.AlumnoId == a.AlumnoId)
                    .Select(x => x.TipoRuta)
                    .FirstOrDefault()
            }).ToList();

            return View(resultado);
        }

        // 📍 Ruta
        public IActionResult Ruta()
        {
            return View();
        }

        // 📝 Asistencia
        public IActionResult Asistencia()
        {
            return View();
        }

        // 📱 Registrar QR
        [HttpPost]
        public IActionResult RegistrarQR([FromBody] string qr)
        {
            var alumno = _context.Alumnos
                .FirstOrDefault(a => a.AlumnoId.ToString() == qr);

            if (alumno == null)
                return Json(new { mensaje = "Alumno no encontrado" });

            var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var autobusId = int.Parse(User.FindFirst("AutobusId")?.Value ?? "0");

            var hoy = DateTime.Now.Date;

            // 🔥 Buscar si ya existe asistencia hoy
            var registro = _context.Asistencias
                .FirstOrDefault(a => a.AlumnoId == alumno.AlumnoId && a.Fecha == hoy);

            string mensaje = "";

            if (registro == null)
            {
                // 🟢 PRIMER ESCANEO = SUBIDA
                registro = new Asistencia
                {
                    AlumnoId = alumno.AlumnoId,
                    AutobusId = autobusId,
                    UsuarioId = usuarioId,
                    Fecha = hoy,
                    TipoRuta = "Entrada",
                    Presente = true,
                    FechaHora = DateTime.Now
                };

                _context.Asistencias.Add(registro);
                mensaje = $"🟢 {alumno.NombreCompleto} SUBIÓ al autobús";
            }
            else if (registro.TipoRuta == "Entrada")
            {
                // 🔴 SEGUNDO ESCANEO = BAJADA
                registro.TipoRuta = "Salida";
                registro.FechaHora = DateTime.Now;

                mensaje = $"🔴 {alumno.NombreCompleto} BAJÓ del autobús";
            }
            else
            {
                // ⚠️ Ya hizo todo
                mensaje = $"⚠️ {alumno.NombreCompleto} ya fue registrado hoy";
            }

            _context.SaveChanges();

            return Json(new { mensaje });
        }
        // 🧾 Generar QR
        public IActionResult GenerarQR(int id)
        {
            var qrGenerator = new QRCodeGenerator();
            var qrData = qrGenerator.CreateQrCode(id.ToString(), QRCodeGenerator.ECCLevel.Q);
            var qrCode = new QRCode(qrData);
            var qrImage = qrCode.GetGraphic(30);

            using (var ms = new MemoryStream())
            {
                qrImage.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                return File(ms.ToArray(), "image/png");
            }
        }
    }


}