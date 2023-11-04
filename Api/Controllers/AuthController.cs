using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Data;
using System.Text;
using Api.Logica;
using Api.Entidades;
using Api.EF;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{

    private readonly IConfiguration _configuration;
    private IUsuarioServicio _usuariosServicio;
    private IIngresoServicio _ingresoServicio;
    //private readonly TimeZoneInfo _timeZone;


    public AuthController(IConfiguration configuration, IUsuarioServicio uServicio, IIngresoServicio iServicio)//, IUserService userService)
    {
        _configuration = configuration;
        //_userService = userService;
        //_timeZone = timeZone;
        _usuariosServicio = uServicio;
        _ingresoServicio = iServicio;
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
