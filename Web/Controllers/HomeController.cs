using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using Web.Entidades;
using Web.Models;

namespace Web.Controllers
{
    public class HomeController : ControllerGenerico
    {

        public HomeController(IConfiguration configuration) : base(configuration)
        {

        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            addRol();
            if (getRolFromToken() == "admin")
            {
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["jwt"]);
                HttpResponseMessage response = await _client.GetAsync("Home/get-usuarios");
                if (response.IsSuccessStatusCode)
                {
                    //Aca se "deberia" usar ReadAsStringAsync pero como recibe un JSON pone la respuesta entre comillas y rompe todo
                    List<UsuarioTemplate>? usuarios = await response.Content.ReadFromJsonAsync<List<UsuarioTemplate>>();
                    return View("ListadoUsuarios", usuarios);
                }
                if (response.StatusCode.Equals(HttpStatusCode.Unauthorized))
                {
                    TempData["error"] = "No Autorizado. ";
                }
                TempData["error"] += "No se pudo obtener la lista de usuarios.";
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
                HttpResponseMessage response = await _client.GetAsync("Home/get-info-usuario?id=" + Id);
                if (response.IsSuccessStatusCode)
                {
                    //Aca se "deberia" usar ReadAsStringAsync pero como recibe un JSON pone la respuesta entre comillas y rompe todo
                    UsuarioTemplate? usuario = await response.Content.ReadFromJsonAsync<UsuarioTemplate>();

                    await AgregarRolesAlViewBagAsync();
                    return View(usuario);
                }
                if (response.StatusCode.Equals(HttpStatusCode.NotFound))
                {
                    TempData["error"] = $"No se encontro el usuario con Id {Id}";
                    return RedirectToAction("Index");
                }
                if (response.StatusCode.Equals(HttpStatusCode.Unauthorized))
                {
                    TempData["error"] = "No Autorizado. ";
                }
                TempData["error"] += $"No se pudo obtener el usuario.";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["error"] = "Se necesita permiso de admin para realizar esta accion";
                return RedirectToAction("Index");
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
                    TempData["OkMsg"] = "El usuario se editó correctamente";

                    return RedirectToAction("Index");
                }
                if (response.StatusCode.Equals(HttpStatusCode.Unauthorized))
                {
                    TempData["error"] = "No Autorizado. ";
                }
                TempData["error"] += "No se pudo editar correctamente, por favor intente de nuevo.";
                await AgregarRolesAlViewBagAsync();
                return View(usuario);
            }
            else
            {
                await AgregarRolesAlViewBagAsync();
                return View(usuario);
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
                    TempData["OkMsg"] = "El usuario se eliminó correctamente";

                    return RedirectToAction("Index", "Home");
                }
                if (response.StatusCode.Equals(HttpStatusCode.Unauthorized))
                {
                    TempData["error"] = "No Autorizado. ";
                }
                TempData["error"] += "No se pudo eliminar, por favor intente de nuevo.";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["error"] = "No se pudo eliminar, no posee permisos";
                return View("Index");
            }
        }


        private async Task AgregarRolesAlViewBagAsync()
        {
            try
            {
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["jwt"]);
                HttpResponseMessage response = await _client.PostAsync("Home/get-roles", null);

                if (response.IsSuccessStatusCode)
                {
                    var rolesJson = await response.Content.ReadAsStringAsync();
                    List<Rol>? roles = JsonConvert.DeserializeObject<List<Rol>>(rolesJson);

                    ViewBag.ListaRoles = roles;
                }
                else
                {
                    if (response.StatusCode.Equals(HttpStatusCode.Unauthorized))
                    {
                        TempData["error"] = "No Autorizado. ";
                    }
                    TempData["error"] += "Error al agregar los roles al view bag";
                    ViewBag.ListaRoles = null;
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                ViewBag.ListaRoles = null;
            }
        }


    }
}