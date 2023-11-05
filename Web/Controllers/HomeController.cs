using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http.Headers;
using Web.Entidades;
using Web.Models;

namespace Web.Controllers
{
    public class HomeController : ControllerGenerico
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<LoginController> logger, IConfiguration configuration) : base(logger, configuration)
        {
            
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            addRol();
            if (getRolFromToken() == "admin")
            {
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["jwt"]);
                HttpResponseMessage response = await _client.PostAsJsonAsync("Home/get-usuarios", "admin");
                if (response.IsSuccessStatusCode)
                {
                    //Aca se "deberia" usar ReadAsStringAsync pero como recibe un JSON pone la respuesta entre comillas y rompe todo
                    List<UsuarioTemplate> usuarios = await response.Content.ReadFromJsonAsync<List<UsuarioTemplate>>();
                    //new { usuarios = JsonConvert.SerializeObject(usuarios) }
                    return View("ListadoUsuarios", usuarios);
                }
                ViewBag.error = "No se pudo obtener la lista de usuarios.";
                return View();
            }
            else
            {
                return View();
            }
        }


        public async Task<ActionResult> ListadoUsuarios(string usuarios)
        {
            var usuariosList = JsonConvert.DeserializeObject<List<UsuarioTemplate>>(usuarios);
            await AgregarRolesAlViewBagAsync();

            return View(usuariosList ?? new List<UsuarioTemplate>());
        }


        private async Task AgregarRolesAlViewBagAsync()
        {

            try
            {
                HttpResponseMessage response = await _client.PostAsJsonAsync("Home/get-roles", "admin");

                if (response.IsSuccessStatusCode)
                {
                    var rolesJson = await response.Content.ReadAsStringAsync();
                    List<Rol> roles = JsonConvert.DeserializeObject<List<Rol>>(rolesJson);

                    ViewBag.ListaRoles = roles;
                }
                else
                {
                    ViewBag.ListaRoles = null; // En caso de que la respuesta no sea exitosa
                }
            }
            catch (Exception ex)
            {
                // Manejo de excepciones
                ViewBag.ListaRoles = null; // Puedes manejar el error de la manera que prefieras
            }
        }
    }
}