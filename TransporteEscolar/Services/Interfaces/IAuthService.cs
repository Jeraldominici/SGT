using TransporteEscolar.ViewModels;
using System.Security.Claims;

namespace TransporteEscolar.Services.Interfaces
{
    public interface IAuthService
    {
        Task<(bool Exitoso, string? Error, ClaimsPrincipal? Principal)>
            ValidarLoginAsync(LoginViewModel modelo, string ip, string userAgent);
    }
}