using Microsoft.AspNetCore.Mvc;
using Api.Logica;
using Api.Entidades;
using Api.EF;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private IUsuarioServicio _usuariosServicio;
        private IIngresoServicio _ingresoServicio;
        private IRolesServicio _rolesServicio;
        //private readonly TimeZoneInfo _timeZone;


        public HomeController(IConfiguration configuration, IUsuarioServicio uServicio, IIngresoServicio iServicio, IRolesServicio rolesServicio)
        {
            _configuration = configuration;
            //_userService = userService;
            //_timeZone = timeZone;
            _usuariosServicio = uServicio;
            _ingresoServicio = iServicio;
            _rolesServicio = rolesServicio;
        }


        [HttpPost("get-usuarios")]
        public ActionResult<List<Usuario>> GetUsuarios()
        {
            try
            {
                List <Usuario> usuarios= _usuariosServicio.Listar();

                return Ok(usuarios);
            }
            catch (Exception ex)
            {

                return BadRequest("Hubo un error al obtener el listado de usuarios.");
            }
        }

        [HttpPost("get-roles")]
        public ActionResult<List<Role>> GetRoles()
        {
            try
            {
                List<Role> roles = _rolesServicio.Listar();

                return Ok(roles);
            }
            catch (Exception ex)
            {

                return BadRequest("Hubo un error al obtener el listado de roles.");
            }
        }
    }
}
