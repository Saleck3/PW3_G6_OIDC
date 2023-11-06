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

        [HttpGet]
        public async Task<IActionResult> Editar(int Id)
        {
            if (getRolFromToken() == "admin")
            {
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["jwt"]);
                HttpResponseMessage response = await _client.PostAsJsonAsync("Home/get-info-usuario", Id);
                if (response.IsSuccessStatusCode)
                {
                    //Aca se "deberia" usar ReadAsStringAsync pero como recibe un JSON pone la respuesta entre comillas y rompe todo
                    UsuarioTemplate usuario = await response.Content.ReadFromJsonAsync<UsuarioTemplate>();

                    await AgregarRolesAlViewBagAsync();
                    return View("Editar", usuario);
                }
                ViewBag.error = "No se pudo obtener la lista de usuarios.";
                return View();
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Editar(UsuarioTemplate usuario)
        {
            addRol();
            if (getRolFromToken() == "admin")
            {
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["jwt"]);
                HttpResponseMessage response = await _client.PostAsJsonAsync("home/editar", usuario);
                if (response.IsSuccessStatusCode)
                {
                    @ViewBag.OkMsg = "El usuario se editó correctamente";

                    return View("Index");
                }
                ViewBag.error = "No se pudo editar correctamente, por favor intente de nuevo.";
                return View();
            }
            else
            {
                return View();
            }
        }


        [HttpGet]
        public async Task<IActionResult> Eliminar(int Id)
        {
            addRol();
            if (getRolFromToken() == "admin")
            {
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["jwt"]);
                HttpResponseMessage response = await _client.PostAsJsonAsync("home/eliminar", Id);
                if (response.IsSuccessStatusCode)
                {
                    @ViewBag.OkMsg = "El usuario se eliminó correctamente";

                    return RedirectToAction("Index", "Home");
                }
                ViewBag.error = "No se pudo eliminar, por favor intente de nuevo.";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.error = "No se pudo eliminar, no posee permisos";
                return View("Index");
            }
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