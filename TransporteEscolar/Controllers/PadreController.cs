using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TransporteEscolar.Controllers
{
    [Authorize(Roles = "Padre")]
    public class PadreController : Controller
    {
        public IActionResult Dashboard()
        {
            ViewBag.NombreCompleto = User.FindFirst(System.Security.Claims.ClaimTypes.GivenName)?.Value;
            return View();
        }
    }
}