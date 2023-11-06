using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using Web.Models;

namespace Web.Controllers
{
    public class LoginController : Controller
    {
        private readonly ILogger<LoginController> _logger;

        public LoginController(ILogger<LoginController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View(new Usuario());
        }

        // Ruta de redirección después de iniciar sesión con Google
        [Authorize(AuthenticationSchemes = "Google")]
        public async Task<IActionResult> GoogleCallback()
        {
            var authenticateResult = await HttpContext.AuthenticateAsync("Google");

            if (!authenticateResult.Succeeded)
            {
                // Manejar el error de autenticación
                return RedirectToAction("Error");
            }

            // Acceder a los datos del usuario autenticado
            var user = authenticateResult.Principal;
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var email = user.FindFirst(ClaimTypes.Email)?.Value;

            // Luego, puedes pasar estos datos a tu API
            // Esto es solo un ejemplo, en tu aplicación, puede depender de cómo quieras usar estos datos

            // Redirigir o mostrar una vista según tus necesidades
            return View("LoggedIn", new { UserId = userId, Email = email });
        }




        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}