using System.ComponentModel.DataAnnotations;

namespace TransporteEscolar.ViewModels
{
    public class AutobusFormViewModel
    {
        public int AutobusId { get; set; }

        [Required(ErrorMessage = "La ficha es obligatoria.")]
        [StringLength(20, ErrorMessage = "Máximo 20 caracteres.")]
        [Display(Name = "Ficha")]
        public string Ficha { get; set; } = string.Empty;

        [Required(ErrorMessage = "La placa es obligatoria.")]
        [StringLength(20, ErrorMessage = "Máximo 20 caracteres.")]
        [Display(Name = "Placa")]
        public string Placa { get; set; } = string.Empty;

        [Required(ErrorMessage = "La capacidad es obligatoria.")]
        [Range(1, 100, ErrorMessage = "La capacidad debe estar entre 1 y 100.")]
        [Display(Name = "Capacidad")]
        public int Capacidad { get; set; } = 30;

        [Display(Name = "Activo")]
        public bool Activo { get; set; } = true;

        public bool EsEdicion => AutobusId > 0;
    }
}