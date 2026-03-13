using System.ComponentModel.DataAnnotations;

namespace TransporteEscolar.ViewModels
{
    /// <summary>
    /// ViewModel para el formulario de inicio de sesión.
    /// Incluye validaciones condicionales para la ficha del autobús.
    /// </summary>
    public class LoginViewModel : IValidatableObject
    {
        [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
        [StringLength(100, ErrorMessage = "Máximo 100 caracteres.")]
        [Display(Name = "Usuario")]
        public string NombreUsuario { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Rol")]
        [Required(ErrorMessage = "Seleccione un rol.")]
        public string Rol { get; set; } = string.Empty;

        [Display(Name = "Ficha del Autobús")]
        [StringLength(20, ErrorMessage = "Máximo 20 caracteres.")]
        public string? FichaAutobus { get; set; }

        public bool RememberMe { get; set; }

        /// <summary>
        /// Validación personalizada: si el rol es Azafata, la ficha es obligatoria.
        /// </summary>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.Equals(Rol, "Azafata", StringComparison.OrdinalIgnoreCase)
                && string.IsNullOrWhiteSpace(FichaAutobus))
            {
                yield return new ValidationResult(
                    "La ficha del autobús es obligatoria para el rol Azafata.",
                    new[] { nameof(FichaAutobus) });
            }
        }
    }
}