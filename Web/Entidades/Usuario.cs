using System;
using System.ComponentModel.DataAnnotations;

namespace Web;
public class Usuario
{
    [Required]
    public string? Email { get; set; }
    [Required]
    public string? Username { get; set; } 
    [Required]
    public string? Password { get; set; }
}
