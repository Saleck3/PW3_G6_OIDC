using System;
using System.Collections.Generic;

namespace Api.EF;

public partial class Ingreso
{
    public int Id { get; set; }

    public DateTime Fecha { get; set; }

    public int UserId { get; set; }

    public virtual Usuario User { get; set; } = null!;
}
