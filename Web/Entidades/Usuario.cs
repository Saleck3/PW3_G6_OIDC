using System.ComponentModel.DataAnnotations;

namespace Web.Entidades
{
    public class Usuario
    {
        [StringLength(25, MinimumLength = 3, ErrorMessage = "La longitud del nombre de usuario debe estar entre 3 y 25 caracteres")]
        [Required(ErrorMessage = "El nombre de usuario es requerido")]
        public string? Username { get; set; }

        [StringLength(250, MinimumLength = 8, ErrorMessage = "La longitud de la clave debe estar entre 8 y 250 caracteres")]
        [Required(ErrorMessage = "La clave es requerida")]
        public string? Password { get; set; }
    }
}
