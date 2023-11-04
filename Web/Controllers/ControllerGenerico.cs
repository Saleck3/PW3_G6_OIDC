using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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
}
