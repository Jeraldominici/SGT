using System.Security.Claims;
using TransporteEscolar.Helpers;          // ← SIN el ".Web"
using TransporteEscolar.Models;
using TransporteEscolar.Repositories.Interfaces;
using TransporteEscolar.Services.Interfaces;
using TransporteEscolar.ViewModels;


namespace TransporteEscolar.Services      // ← SIN el ".Web"
{
    public class AuthService : IAuthService
    {
        private readonly IUsuarioRepository _usuarioRepo;

        public AuthService(IUsuarioRepository usuarioRepo)
            => _usuarioRepo = usuarioRepo;

        public async Task<(bool Exitoso, string? Error, ClaimsPrincipal? Principal)>
            ValidarLoginAsync(LoginViewModel modelo, string ip, string userAgent)
        {
            var usuario = await _usuarioRepo.ObtenerPorNombreUsuarioAsync(modelo.NombreUsuario);

            if (usuario is null)
            {
                await RegistrarBitacora(null, modelo.NombreUsuario, false, ip, userAgent, "Usuario no encontrado");
                return (false, "Credenciales incorrectas.", null);
            }

            if (usuario.Bloqueado)
            {
                await RegistrarBitacora(usuario.UsuarioId, modelo.NombreUsuario, false, ip, userAgent, "Cuenta bloqueada");
                return (false, "Tu cuenta está bloqueada. Contacta al administrador.", null);
            }

            if (!string.Equals(usuario.Rol.Nombre, modelo.Rol, StringComparison.OrdinalIgnoreCase))
            {
                await _usuarioRepo.IncrementarIntentosFallidosAsync(usuario.UsuarioId);
                await RegistrarBitacora(usuario.UsuarioId, modelo.NombreUsuario, false, ip, userAgent, "Rol incorrecto");
                return (false, "Credenciales incorrectas.", null);
            }

            if (!PasswordHelper.VerifyPassword(modelo.Password, usuario.PasswordHash))
            {
                await _usuarioRepo.IncrementarIntentosFallidosAsync(usuario.UsuarioId);
                await RegistrarBitacora(usuario.UsuarioId, modelo.NombreUsuario, false, ip, userAgent, "Contraseña incorrecta");
                return (false, "Credenciales incorrectas.", null);
            }

            if (string.Equals(usuario.Rol.Nombre, "Azafata", StringComparison.OrdinalIgnoreCase))
            {
                if (usuario.Autobus is null ||
                    !string.Equals(usuario.Autobus.Ficha, modelo.FichaAutobus, StringComparison.OrdinalIgnoreCase))
                {
                    await RegistrarBitacora(usuario.UsuarioId, modelo.NombreUsuario, false, ip, userAgent, "Ficha de autobús inválida");
                    return (false, "La ficha del autobús no es válida.", null);
                }
            }

            await _usuarioRepo.ActualizarUltimoAccesoAsync(usuario.UsuarioId);
            await RegistrarBitacora(usuario.UsuarioId, modelo.NombreUsuario, true, ip, userAgent, "Acceso exitoso");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.UsuarioId.ToString()),
                new Claim(ClaimTypes.Name,           usuario.NombreUsuario),
                new Claim(ClaimTypes.GivenName,      usuario.NombreCompleto),
                new Claim(ClaimTypes.Role,           usuario.Rol.Nombre),
            };

            if (usuario.Autobus is not null)
                claims.Add(new Claim("AutobusId", usuario.AutobusId.ToString()!));

            var identity = new ClaimsIdentity(claims, "CookieAuth");
            var principal = new ClaimsPrincipal(identity);

            return (true, null, principal);
        }

        private async Task RegistrarBitacora(
            int? usuarioId, string nombreUsuario, bool exitoso,
            string ip, string userAgent, string detalle)
        {
            await _usuarioRepo.RegistrarBitacoraAsync(new BitacoraAcceso
            {
                UsuarioId = usuarioId,
                NombreUsuario = nombreUsuario,
                FechaHora = DateTime.UtcNow,
                Exitoso = exitoso,
                DireccionIP = ip,
                UserAgent = userAgent,
                Detalle = detalle
            });
        }
    }
}