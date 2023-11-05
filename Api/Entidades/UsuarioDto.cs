namespace Api.Entidades
{
    public class UsuarioDto
    {
        public int? Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int? Rol { get; set; } 

    }
}
