using TransporteEscolar.Models;

namespace TransporteEscolar.Repositories.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<Usuario?> ObtenerPorNombreUsuarioAsync(string nombreUsuario);
        Task ActualizarUltimoAccesoAsync(int usuarioId);
        Task IncrementarIntentosFallidosAsync(int usuarioId);
        Task ResetearIntentosFallidosAsync(int usuarioId);
        Task RegistrarBitacoraAsync(BitacoraAcceso bitacora);
    }
}