using System;
using System.Collections.Generic;

namespace PUNDERO.Models;

public partial class Coordinator
{
    public int IdCoordinator { get; set; }

    public string Qualification { get; set; } = null!;

    public string Description { get; set; } = null!;

    public int IdAccount { get; set; }

    public virtual Account IdAccountNavigation { get; set; } = null!;
}
