using System;
using System.Collections.Generic;

namespace PUNDERO.Models;

public partial class Client
{
    public int IdClient { get; set; }

    public int? IdAccount { get; set; }

    public virtual Account? IdAccountNavigation { get; set; }

    public virtual ICollection<Store> Stores { get; set; } = new List<Store>();
}
