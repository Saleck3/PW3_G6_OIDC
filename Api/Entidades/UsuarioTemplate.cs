using Api.EF;

namespace Api.Entidades
{
    public class UsuarioTemplate
    {
        public int? Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string? Rol { get; set; }
        public int? IdRol { get; set; }

        public UsuarioTemplate() { }

        public UsuarioTemplate(int? id, string username, string? rol, int? idRol)
        {
            Id = id;
            Username = username;
            Rol = rol;
            IdRol = idRol;
        }

        public UsuarioTemplate(Usuario usuario)
        {

            Id = usuario.Id;
            Username = usuario.Username;
            if (usuario.RolNavigation != null)
            {
                Rol = usuario.RolNavigation.Nombre;
                IdRol = usuario.RolNavigation.Id;
            }
        }

    }
}
