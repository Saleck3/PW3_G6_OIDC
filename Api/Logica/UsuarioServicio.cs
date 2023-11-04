using Api.EF;
using Api.Entidades;

namespace Api.Logica
{
    public interface IUsuarioServicio
    {
        //List<UsuarioDt> Listar();
        void Crear(Usuario user);
        List<Usuario> Filtrar(int? id);
        List<Usuario> Filtrar(String? username);
        void Eliminar(int id);
    }

    public class UsuarioServicio : IUsuarioServicio
    {
        private WebContext _contexto;

        public UsuarioServicio(WebContext contexto)
        {
            _contexto = contexto;
        }

        /*public List<UsuarioDt> Listar()
        {
            return _contexto.UsuarioDts.Include(t => t.Id).ToList();
        }*/
        public void Crear(Usuario usuario)
        {
            _contexto.Usuarios.Add(usuario);
            _contexto.SaveChanges();
        }

        public List<Usuario> Filtrar(int? idCadena)
        {
            return _contexto.Usuarios.Where(s => s.Id == idCadena).ToList();
        }

        public List<Usuario> Filtrar(String? userName)
        {
            return _contexto.Usuarios.Where(s => s.Username == userName).ToList();
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
    }
}
