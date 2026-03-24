using System.ComponentModel.DataAnnotations;

namespace TransporteEscolar.ViewModels
{
    public class MensajeManualViewModel
    {
        [Required(ErrorMessage = "El título es obligatorio.")]
        [StringLength(100)]
        public string Titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El mensaje es obligatorio.")]
        [StringLength(500)]
        public string Mensaje { get; set; } = string.Empty;

        public bool EnviarATodos { get; set; } = true;
        public List<int> UsuariosSeleccionados { get; set; } = new();
    }
}