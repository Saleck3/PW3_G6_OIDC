using System;
using System.Collections.Generic;

namespace Api.EF;

public partial class Role
{
    public int Id { get; set; }

    public string? Nombre { get; set; }

    public virtual Usuario? Usuario { get; set; }
}
