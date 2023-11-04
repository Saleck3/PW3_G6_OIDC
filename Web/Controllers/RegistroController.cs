using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Web.Models;
using Web.Entidades;

namespace Web.Controllers
{
    public class RegistroController : ControllerGenerico
    {
        public RegistroController(ILogger<LoginController> logger, IConfiguration configuration) : base(logger, configuration)
        {
        }

        public IActionResult Registro()
        {
            return View(new Usuario());
        }

        [HttpPost]
        public async Task<IActionResult> Agregar(Usuario usuario)
        {
            if (!ModelState.IsValid)
            {
                return View(usuario);
            }

            HttpResponseMessage responseCreacion = await _client.PostAsJsonAsync("Auth/Registro", usuario);
            responseCreacion.EnsureSuccessStatusCode();

            HttpResponseMessage response = await _client.PostAsJsonAsync("Auth/login", usuario);
            response.EnsureSuccessStatusCode();

            //Aca se "deberia" usar ReadAsStringAsync pero como recibe un JSON pone la respuesta entre comillas y rompe todo
            string token = await response.Content.ReadFromJsonAsync<string>();
            Response.Cookies.Append("jwt", token, _cookieOptions);
            return RedirectToAction("Index", "Home");
        }
    }
}