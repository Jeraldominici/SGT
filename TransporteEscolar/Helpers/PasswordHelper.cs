using BCrypt.Net;

namespace TransporteEscolar.Helpers   // ← SIN el ".Web"
{
    /// <summary>
    /// Utilidad para hashear y verificar contraseñas con BCrypt.
    /// </summary>
    public static class PasswordHelper
    {
        private const int WorkFactor = 12;

        public static string HashPassword(string password)
            => BCrypt.Net.BCrypt.HashPassword(password, WorkFactor);

        public static bool VerifyPassword(string password, string hash)
            => BCrypt.Net.BCrypt.Verify(password, hash);
    }
}