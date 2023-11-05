using Api.EF;

namespace Api.Logica;

public interface IRolesServicio
{
    int getIdRolAdmin();
    int getIdRolUser();
    string getNombreRolAdmin();
    string getNombreRolUser();
    List<Role> Listar();
}

public class RolesServicio : IRolesServicio
{
    private WebContext _contexto;

    public RolesServicio(WebContext contexto)
    {
        _contexto = contexto;
    }

    public List<Role> Listar()
    {
        return _contexto.Roles.ToList();
    }
    public int getIdRolUser()
    {
        return _contexto.Roles.Where(s => s.Nombre == "user").FirstOrDefault().Id;
    }
    public int getIdRolAdmin()
    {
        return _contexto.Roles.Where(s => s.Nombre == "admin").FirstOrDefault().Id;
    }

    public string getNombreRolUser()
    {
        return _contexto.Roles.Where(s => s.Nombre == "user").FirstOrDefault().Nombre;
    }
    public string getNombreRolAdmin()
    {
        return _contexto.Roles.Where(s => s.Nombre == "admin").FirstOrDefault().Nombre;
    }
}
