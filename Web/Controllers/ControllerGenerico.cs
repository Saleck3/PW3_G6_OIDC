using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using Web.Entidades;

namespace Web.Controllers;

public abstract class ControllerGenerico : Controller
{
    protected readonly ILogger<LoginController> _logger;
    protected readonly IConfiguration _configuration;
    protected HttpClient _client = new HttpClient();
    protected CookieOptions _cookieOptions = new CookieOptions { HttpOnly = true, Expires = DateTime.Now.AddMinutes(15) };
    public ControllerGenerico(ILogger<LoginController> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
        // Update port # in the following line.
        _client.BaseAddress = new Uri(_configuration.GetSection("urls").GetSection("api").Value);
        _client.DefaultRequestHeaders.Accept.Clear();
        _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }
    protected void addRol()
    {
        //leo JWT y saco el rol
        string jwtCookie = Request.Cookies["jwt"];
        if (jwtCookie != null)
        {
            JwtSecurityToken jwt = new JwtSecurityTokenHandler().ReadJwtToken(jwtCookie);
            string rol = jwt.Claims.First(c => c.Type == "roles").Value;
            //agrego el rol a la vista
            ViewBag.rol = rol;
            Response.Cookies.Append("rol", rol, _cookieOptions);
        }

    }
}
