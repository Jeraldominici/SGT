using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TransporteEscolar.Controllers
{
    [Authorize(Roles = "Azafata")]
    public class AzafataController : Controller
    {
        public IActionResult Dashboard()
        {
            ViewBag.NombreCompleto = User.FindFirst(System.Security.Claims.ClaimTypes.GivenName)?.Value;
            ViewBag.AutobusId = User.FindFirst("AutobusId")?.Value;
            return View();
        }
    }
}