using Api.Entidades;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        public static Usuario usuario = new Usuario();
        private readonly IConfiguration _configuration;
        //private readonly TimeZoneInfo _timeZone;


        public AuthController(IConfiguration configuration)//, IUserService userService)
        {
            _configuration = configuration;
            //_userService = userService;
            //_timeZone = timeZone;

        }

        [HttpPost("Registro")]
        public async Task<ActionResult<Usuario>> Registro(UsuarioDto request)
        {
            CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);
            usuario.Username = request.Username;
            usuario.PasswordSalt = passwordSalt;
            usuario.PasswordHash = passwordHash;
            return Ok(usuario);

        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(UsuarioDto request)
        {
            if (usuario.Username != request.Username)
            {
                return BadRequest("User not found.");
            }

            if (!VerifyPasswordHash(request.Password, usuario.PasswordHash, usuario.PasswordSalt))
            {
                return BadRequest("Wrong password.");
            }

            string token = CreateToken(usuario);

            var refreshToken = GenerateRefreshToken();
            SetRefreshToken(refreshToken);

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

        private void SetRefreshToken(RefreshToken nuevoRefreshToken)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = nuevoRefreshToken.Expires,
            };

            Response.Cookies.Append("refreshToken", nuevoRefreshToken.Token, cookieOptions);
            usuario.RefreshToken = nuevoRefreshToken.Token;
            usuario.TokenCreated = nuevoRefreshToken.Created;
            usuario.TokenExpires = nuevoRefreshToken.Expires;
        }

 
        [HttpPost("refresh-token")]
        public async Task<ActionResult<string>> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (!usuario.RefreshToken.Equals(refreshToken))
            {
                return Unauthorized("Refresh Token inválido");
            }else if(usuario.TokenExpires < DateTime.Now)
            {
                return Unauthorized("El token expiró");
            }

            string token = CreateToken(usuario);
            var nuevoRefreshToken = GenerateRefreshToken();
            SetRefreshToken(nuevoRefreshToken);
            return Ok(token);
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }

        private string CreateToken(Usuario usuario)
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
}
