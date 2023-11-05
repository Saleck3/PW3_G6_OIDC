using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Web.Models;

namespace Web.Controllers
{
    public class HomeController : ControllerGenerico
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<LoginController> logger, IConfiguration configuration) : base(logger, configuration)
        {
            
        }

        public IActionResult Index()
        {
            addRol();
            return View();
        }
    }
}