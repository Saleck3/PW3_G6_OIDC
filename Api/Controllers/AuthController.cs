using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Data;
using System.Text;
using Api.Logica;
using Api.Entidades;
using Api.EF;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using System.Net.Http.Headers;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private IUsuarioServicio _usuariosServicio;
    private IIngresoServicio _ingresoServicio;
    //private readonly TimeZoneInfo _timeZone;


    public AuthController(IConfiguration configuration, IUsuarioServicio uServicio, IIngresoServicio iServicio, IHttpClientFactory httpClientFactory)//, IUserService userService)
    {
        _configuration = configuration;
        //_userService = userService;
        //_timeZone = timeZone;
        _usuariosServicio = uServicio;
        _ingresoServicio = iServicio;
        _httpClientFactory = httpClientFactory;
    }

    [HttpPost("Registro")]
    public async Task<ActionResult<Usuario>> Registro(UsuarioDto request)
    {
        try
        {
            Usuario user = new Usuario();
            CreatePasswordHash(request.Password, ref user);
            user.Username = request.Username;
            _usuariosServicio.Crear(user);
            return Ok(user);
        }
        catch (Exception)
        {
            return BadRequest("User Exists.");
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<string>> Login(UsuarioDto request)
    {
        Usuario user = (Usuario)_usuariosServicio.Filtrar(request.Username).First();
        if (user == null)
        {
            return BadRequest("User not found.");
        }

        if (!VerifyPasswordHash(request.Password, ref user))
        {
            return BadRequest("Wrong password.");
        }

        RegistrarIngreso(ref user);

        string token = CreateToken(ref user);

        var refreshToken = GenerateRefreshToken();
        SetRefreshToken(refreshToken, ref user);

        return Ok(token);
    }

    [AllowAnonymous]
    [HttpGet("login-google")]
    public IActionResult LoginWithGoogle()
    {
        var properties = new AuthenticationProperties
        {
            RedirectUri = Url.Action("GoogleCallback"),
        };

        return Challenge(properties, "Google");
    }

    [AllowAnonymous]
    [HttpGet("google-callback")]
    public async Task<IActionResult> GoogleCallback()
    {
        var authenticateResult = await HttpContext.AuthenticateAsync("Google");

        if (!authenticateResult.Succeeded)
        {
            return BadRequest("La autenticación con Google falló.");
        }

        var accessToken = authenticateResult.Properties.GetTokenValue("access_token");

        if (string.IsNullOrEmpty(accessToken))
        {
            return BadRequest("Token de acceso no encontrado.");
        }

        // Realiza llamadas a la API de Google con el token de acceso (ejemplo a continuación).
        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await client.GetAsync("https://www.googleapis.com/oauth2/v2/userinfo");

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            return Ok(content);
        }
        else
        {
            return BadRequest("Error al acceder a la API de Google.");
        }

        return Ok("Iniciaste sesión con Google.");
    }


    private RefreshToken GenerateRefreshToken()
    {
        var refreshToken = new RefreshToken
        {
            Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
            Expires = DateTime.Now.AddMinutes(2),
            Created = DateTime.Now
        };

        return refreshToken;
    }

    private void SetRefreshToken(RefreshToken nuevoRefreshToken, ref Usuario user)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Expires = nuevoRefreshToken.Expires,
        };

        Response.Cookies.Append("refreshToken", nuevoRefreshToken.Token, cookieOptions);
        user.Refreshtoken = nuevoRefreshToken.Token;
        user.Tokencreated = nuevoRefreshToken.Created;
        user.Tokenexpires = nuevoRefreshToken.Expires;
    }


    [HttpPost("refresh-token")]
    public async Task<ActionResult<string>> RefreshToken(Usuario user)
    {
        var refreshToken = Request.Cookies["refreshToken"];
        if (!user.Refreshtoken.Equals(refreshToken))
        {
            return Unauthorized("Refresh Token inválido");
        }
        else if (user.Tokenexpires < DateTime.Now)
        {
            return Unauthorized("El token expiró");
        }

        string token = CreateToken(ref user);
        var nuevoRefreshToken = GenerateRefreshToken();
        SetRefreshToken(nuevoRefreshToken, ref user);
        return Ok(token);
    }

    private void CreatePasswordHash(string password, ref Usuario usuario)
    {

        using (var hmac = new HMACSHA512())
        {
            usuario.Passwordsalt = hmac.Key;
            usuario.Passwordhash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        }

        usuario.Password = Encoding.UTF8.GetString(usuario.Passwordhash);
    }

    private bool VerifyPasswordHash(string password, ref Usuario user)
    {
        using (var hmac = new HMACSHA512(user.Passwordsalt))
        {
            var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return computedHash.SequenceEqual(user.Passwordhash);
        }
    }

    private void RegistrarIngreso(ref Usuario user)
    {
        Ingreso ingreso = new Ingreso();
        ingreso.UserId = user.Id;
        ingreso.User = user;
        _ingresoServicio.Crear(ingreso);
    }

    private string CreateToken(ref Usuario usuario)
    {
        List<Claim> claims = new List<Claim>
        {
            new Claim("nombre", usuario.Username),
            new Claim("roles", "Admin")
        };
        var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
            _configuration.GetSection("Token").Value));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddMinutes(2),
            signingCredentials: creds);

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        return jwt;
    }
}
