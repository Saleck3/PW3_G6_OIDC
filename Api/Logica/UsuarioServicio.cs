﻿using Api.EF;
using Api.Entidades;
using Microsoft.EntityFrameworkCore;

namespace Api.Logica;

public interface IUsuarioServicio
{
    //List<UsuarioDt> Listar();
    void Crear(Usuario user);
    public List<Usuario> Listar();
    public Usuario? FiltrarUsuario(int? IdUsuario);
    UsuarioTemplate FiltrarUsuarioTemplate(int IdUsuario);
    Usuario Filtrar(String? username);
    Boolean Eliminar(int id, string usuarioActual);
    public string getNombreRol(Usuario usuario);
    List<UsuarioTemplate> ListarUsuariosTemplate();
    UsuarioTemplate Editar(UsuarioTemplate usuario);
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
        return _contexto.Usuarios.Include(t => t.RolNavigation).ToList();
    }

    public List<UsuarioTemplate> ListarUsuariosTemplate()
    {
        List<Usuario> listado = _contexto.Usuarios.Include(t => t.RolNavigation).ToList();
        List<UsuarioTemplate> resultado = new List<UsuarioTemplate>();
        foreach (var usuario in listado)
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

    public WebContext Get_contexto()
    {
        return _contexto;
    }

    public UsuarioTemplate FiltrarUsuarioTemplate(int IdUsuario)
    {
        var usuario = _contexto.Usuarios.Include(t => t.RolNavigation).FirstOrDefault(u => u.Id == IdUsuario);

        if (usuario != null)
        {
            UsuarioTemplate usuarioTemplate = new UsuarioTemplate(usuario);
            return usuarioTemplate;
        }

        return null; // Maneja el caso donde el usuario no se encuentra en la base de datos
    }

    public Usuario? FiltrarUsuario(int? IdUsuario)
    {
        if (IdUsuario == null)
        {
            return null;
        }
        Usuario usuario = _contexto.Usuarios.Include(t => t.RolNavigation).FirstOrDefault(u => u.Id == IdUsuario);

        if (usuario != null)
        {
            return usuario;
        }

        return null; // Maneja el caso donde el usuario no se encuentra en la base de datos
    }

    public Usuario Filtrar(String? userName)
    {
        return _contexto.Usuarios.Where(s => s.Username == userName).FirstOrDefault();
    }

    public Boolean Eliminar(int idUsuario, string usuarioActual)
    {
        Usuario usuario = FiltrarUsuario(idUsuario);

        if (usuario != null && usuarioActual != usuario.Username)
        {
            // Eliminar los ingresos asociados al usuario
            _contexto.Ingresos.RemoveRange(usuario.Ingresos);

            _contexto.SaveChanges();

            // Eliminar el usuario
            _contexto.Usuarios.Remove(usuario);

            _contexto.SaveChanges();
            return true;
        }
        return false;
    }

    public string getNombreRol(Usuario usuario)
    {
        return _contexto.Roles.Where(s => s.Id == usuario.Rol).FirstOrDefault().Nombre;
    }


    public UsuarioTemplate Editar(UsuarioTemplate usuario)
    {

        var usuarioEncontrado = FiltrarUsuario(usuario.Id);


        if (usuarioEncontrado != null)
        {
            usuarioEncontrado.Username = usuario.Username;
            usuarioEncontrado.RolNavigation = _contexto.Roles.FirstOrDefault(r => r.Nombre == usuario.Rol);
            _contexto.SaveChanges();

            return new UsuarioTemplate(usuarioEncontrado);
        }
        return null;
    }

}
