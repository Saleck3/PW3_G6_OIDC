using Api.EF;
using Api.Entidades;
using Microsoft.EntityFrameworkCore;

namespace Api.Logica
{
    public interface IUsuarioServicio
    {
        //List<UsuarioDt> Listar();
        void Crear(Usuario user);
        public List<Usuario> Listar();
        Usuario Filtrar(int? id);
        Usuario Filtrar(String? username);
        void Eliminar(int id);
        public string getNombreRol(Usuario usuario);
        List<UsuarioTemplate> ListarUsuariosTemplate();
    }

    public class UsuarioServicio : IUsuarioServicio
    {
        private WebContext _contexto;

        public UsuarioServicio(WebContext contexto)
        {
            _contexto = contexto;
        }

        public List<Usuario> Listar()
        {
            return _contexto.Usuarios.Include(t=> t.RolNavigation).ToList();
        }

        public List<UsuarioTemplate> ListarUsuariosTemplate()
        {
             List <Usuario> listado = _contexto.Usuarios.Include(t => t.RolNavigation).ToList();
             List <UsuarioTemplate> resultado = new List<UsuarioTemplate>();
            foreach(var usuario in listado)
            {
                resultado.Add(new UsuarioTemplate(usuario));
            }

            return resultado;
        }
        public void Crear(Usuario usuario)
        {
            _contexto.Usuarios.Add(usuario);
            _contexto.SaveChanges();
        }

        public Usuario Filtrar(int? idCadena)
        {
            return _contexto.Usuarios.Where(s => s.Id == idCadena).First();
        }

        public Usuario Filtrar(String? userName)
        {
            return _contexto.Usuarios.Where(s => s.Username == userName).FirstOrDefault();
        }

        public void Eliminar(int id)
        {
            var sucursal = _contexto.Usuarios.Find(id);
            if (sucursal != null)
            {
                _contexto.Usuarios.Remove(sucursal);
                _contexto.SaveChanges();
            }
        }

        public string getNombreRol(Usuario usuario)
        {
            return _contexto.Roles.Where(s => s.Id == usuario.Rol).FirstOrDefault().Nombre;
        }

    }
}
