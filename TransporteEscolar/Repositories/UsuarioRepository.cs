using Microsoft.EntityFrameworkCore;
using TransporteEscolar.Data;
using TransporteEscolar.Models;
using TransporteEscolar.Repositories.Interfaces;

namespace TransporteEscolar.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly AppDbContext _context;

        public UsuarioRepository(AppDbContext context)
            => _context = context;

        public async Task<Usuario?> ObtenerPorNombreUsuarioAsync(string nombreUsuario)
            => await _context.Usuarios
                .Include(u => u.Rol)
                .Include(u => u.Autobus)
                .FirstOrDefaultAsync(u =>
                    u.NombreUsuario == nombreUsuario && u.Activo);

        public async Task ActualizarUltimoAccesoAsync(int usuarioId)
        {
            var usuario = await _context.Usuarios.FindAsync(usuarioId);
            if (usuario is null) return;
            usuario.UltimoAcceso = DateTime.UtcNow;
            usuario.IntentosFallidos = 0;
            await _context.SaveChangesAsync();
        }

        public async Task IncrementarIntentosFallidosAsync(int usuarioId)
        {
            var usuario = await _context.Usuarios.FindAsync(usuarioId);
            if (usuario is null) return;
            usuario.IntentosFallidos++;
            if (usuario.IntentosFallidos >= 5)
                usuario.Bloqueado = true;
            await _context.SaveChangesAsync();
        }

        public async Task ResetearIntentosFallidosAsync(int usuarioId)
        {
            var usuario = await _context.Usuarios.FindAsync(usuarioId);
            if (usuario is null) return;
            usuario.IntentosFallidos = 0;
            await _context.SaveChangesAsync();
        }

        public async Task RegistrarBitacoraAsync(BitacoraAcceso bitacora)
        {
            _context.BitacoraAccesos.Add(bitacora);
            await _context.SaveChangesAsync();
        }
    }
}