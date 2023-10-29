using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Web.Models;

namespace Web.Controllers
{
    public class RegistroController : Controller
    {
        
        public IActionResult Registro()
        {
            return View(new Usuario());
        }

        [HttpPost]
        public IActionResult Agregar(Usuario usuario) {

            return View();
        }
    }
}