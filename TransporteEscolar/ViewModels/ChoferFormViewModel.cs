using System.ComponentModel.DataAnnotations;

namespace TransporteEscolar.ViewModels
{
    public class ChoferFormViewModel
    {
        public int ChoferId { get; set; }

        [Required(ErrorMessage = "El nombre completo es obligatorio.")]
        [StringLength(200, ErrorMessage = "Máximo 200 caracteres.")]
        [Display(Name = "Nombre Completo")]
        public string NombreCompleto { get; set; } = string.Empty;

        [StringLength(20, ErrorMessage = "Máximo 20 caracteres.")]
        [Display(Name = "Teléfono")]
        public string? Telefono { get; set; }

        [Required(ErrorMessage = "El DUI es obligatorio.")]
        [StringLength(20, ErrorMessage = "Máximo 20 caracteres.")]
        [Display(Name = "DUI / Cédula")]
        public string DUI { get; set; } = string.Empty;

        [Required(ErrorMessage = "La licencia es obligatoria.")]
        [StringLength(30, ErrorMessage = "Máximo 30 caracteres.")]
        [Display(Name = "Licencia de Conducir")]
        public string Licencia { get; set; } = string.Empty;

        [Display(Name = "Foto")]
        public IFormFile? FotoArchivo { get; set; }

        public string? FotoUrlActual { get; set; }

        [Display(Name = "Activo")]
        public bool Activo { get; set; } = true;

        public bool EsEdicion => ChoferId > 0;
    }
}