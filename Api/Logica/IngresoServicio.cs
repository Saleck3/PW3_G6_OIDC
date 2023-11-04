using Api.EF;
using Api.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Api.Logica;

public interface IIngresoServicio
{
    List<Ingreso> Listar();
    void Crear(Ingreso user);
    List<Ingreso> Filtrar(int? id);
    void Eliminar(int id);
}


public class IngresoServicio : IIngresoServicio
{

    private WebContext _contexto;

    public IngresoServicio(WebContext contexto)
    {
        _contexto = contexto;
    }
    
    public List<Ingreso> Listar()
    {
        return _contexto.Ingresos.Include(t => t.Id).ToList();
    }
    public void Crear(Ingreso ingreso)
    {
        _contexto.Ingresos.Add(ingreso);
        _contexto.SaveChanges();
    }

    public List<Ingreso> Filtrar(int? idUsuario)
    {
        return _contexto.Ingresos.Where(s => s.Id == idUsuario).ToList();
    }

    public void Eliminar(int id)
    {
        var ingreso = _contexto.Ingresos.Find(id);
        if (ingreso != null)
        {
            _contexto.Ingresos.Remove(ingreso);
            _contexto.SaveChanges();
        }
    }
}



    




