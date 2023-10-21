using Api.Entidades;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
//using System.IdentityModel.Tokens.Jwt;

namespace Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        public static Usuario usuario = new Usuario();
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)//, IUserService userService)
        {
            _configuration = configuration;
            //_userService = userService;
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

            /*var refreshToken = GenerateRefreshToken();
            SetRefreshToken(refreshToken);*/

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
                new Claim("rol", "Admin")
            };
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                _configuration.GetSection("Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }
    }
}
