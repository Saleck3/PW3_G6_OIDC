using Microsoft.AspNetCore.Mvc;
using Api.Logica;
using Api.Entidades;
using Api.EF;
using Microsoft.AspNetCore.Authorization;

namespace Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(Roles = "admin")]
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


        [HttpGet("get-usuarios")]
        public ActionResult<List<UsuarioTemplate>> GetUsuarios()
        {
            try
            {
                List<UsuarioTemplate> usuarios = _usuariosServicio.ListarUsuariosTemplate();

                return Ok(usuarios);
            }
            catch (Exception ex)
            {
                return BadRequest("Hubo un error al obtener el listado de usuarios." + ex.Message);
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
                return BadRequest("Hubo un error al obtener el listado de roles. " + ex.Message);
            }
        }

        [HttpGet("get-info-usuario")]
        public ActionResult<UsuarioTemplate> GetUsuario(int Id)
        {
            try
            {
                UsuarioTemplate usuario = _usuariosServicio.FiltrarUsuarioTemplate(Id);

                if (usuario == null)
                {
                    return NotFound();
                }
                return Ok(usuario);
            }
            catch (Exception ex)
            {
                return BadRequest("Hubo un error al obtener el usuario. " + ex.Message);
            }
        }

        [HttpPost("editar")]
        public ActionResult<string> Editar([FromBody] UsuarioTemplate usuario)
        {
            try
            {
                UsuarioTemplate usuarioEditado = _usuariosServicio.Editar(usuario);

                if (usuarioEditado != null)
                {
                    return Ok("El usuario se editó correctamente");
                }

                return BadRequest("Hubo un error al editar el usuario.");
            }
            catch (Exception ex)
            {
                return BadRequest("Hubo un error al obtener el usuario." + ex.Message);
            }
        }

        [HttpPost("eliminar")]
        public ActionResult<string> Eliminar([FromBody] int Id)
        {
            try
            {
                var nombreUsuarioClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "nombre")?.Value;
                Boolean usuarioEliminadoOk = _usuariosServicio.Eliminar(Id, usuarioActual: nombreUsuarioClaim);

                if (usuarioEliminadoOk)
                {
                    return Ok("El usuario se eliminó correctamente");
                }

                return BadRequest("Hubo un error al eliminar el usuario.");
            }
            catch (Exception ex)
            {
                return BadRequest("Hubo un error al eliminar el usuario. " + ex.Message);
            }
        }

    }
}
