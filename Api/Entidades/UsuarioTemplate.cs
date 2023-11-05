using Api.EF;

namespace Api.Entidades
{
    public class UsuarioTemplate
    {
        public int? Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string? Rol { get; set; } 


        public UsuarioTemplate(Usuario usuario) {

            Id = usuario.Id;
            Username = usuario.Username;
            Rol = usuario.IdNavigation.Nombre;     }

    }
}
