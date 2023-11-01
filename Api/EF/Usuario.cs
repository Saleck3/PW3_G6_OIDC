using System;
using System.Collections.Generic;

namespace Api.EF;

public partial class Usuario
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public byte[]? Passwordhash { get; set; }

    public byte[]? Passwordsalt { get; set; }

    public string? Refreshtoken { get; set; }

    public DateTime? Tokencreated { get; set; }

    public DateTime? Tokenexpires { get; set; }

    public string? Rol { get; set; }
}
