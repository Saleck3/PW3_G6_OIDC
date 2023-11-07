using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Web.Models;
using Web.Entidades;

namespace Web.Controllers
{
    public class RegistroController : ControllerGenerico
    {
        public RegistroController( IConfiguration configuration) : base( configuration)
        {
        }

        public IActionResult Registro()
        {
            return View(new Usuario());
        }

        [HttpPost]
        public async Task<IActionResult> Registro(Usuario usuario)
        {
            if (!ModelState.IsValid)
            {
                return View(usuario);
            }

            HttpResponseMessage responseCreacion = await _client.PostAsJsonAsync("Auth/Registro", usuario);
            if (responseCreacion.IsSuccessStatusCode)
            {
                HttpResponseMessage response = await _client.PostAsJsonAsync("Auth/login", usuario);
                if (responseCreacion.IsSuccessStatusCode)
                {
                    //Aca se "deberia" usar ReadAsStringAsync pero como recibe un JSON pone la respuesta entre comillas y rompe todo
                    string token = await response.Content.ReadFromJsonAsync<string>();
                    Response.Cookies.Append("jwt", token, _cookieOptions);
                    return RedirectToAction("Index", "Home");
                }
                TempData["error"] = "No se generar un token para el usuario";
                return View(usuario);
            }
            TempData["error"] = "No se pudo crear el usuario ";
            return View(usuario);
        }
    }
}