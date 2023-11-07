using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using Web.Entidades;

namespace Web.Controllers;

public abstract class ControllerGenerico : Controller
{

    protected readonly IConfiguration _configuration;
    protected HttpClient _client = new HttpClient();
    protected CookieOptions _cookieOptions = new CookieOptions { HttpOnly = true, Expires = DateTime.Now.AddMinutes(15) };
    public ControllerGenerico(IConfiguration configuration)
    {
        _configuration = configuration;
        // Update port # in the following line.
        _client.BaseAddress = new Uri(_configuration.GetSection("urls").GetSection("api").Value);
        _client.DefaultRequestHeaders.Accept.Clear();
        _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }
    protected void addRol()
    {
        ViewBag.rol = getRolFromToken();
    }

    protected string getRolFromToken()
    {
        JwtSecurityToken jwt = getJWTFromCookie();
        return jwt.Claims.First(c => c.Type == "roles").Value;
    }
    protected JwtSecurityToken getJWTFromCookie()
    {
        //leo JWT y saco el rol
        string jwtCookie = Request.Cookies["jwt"];
        if (jwtCookie != null)
        {
            return new JwtSecurityTokenHandler().ReadJwtToken(jwtCookie);
        }
        else
        {
            return null;
        }

    }
}
