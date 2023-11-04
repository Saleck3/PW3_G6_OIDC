using System.ComponentModel.DataAnnotations;

namespace Web.Entidades
{
    public class Usuario
    {
        [Required(ErrorMessage = "El nombre de usuario es requerido")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "La clave es requerida")]
        public string? Password { get; set; }
    }
}
