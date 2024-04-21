using System;
using System.Collections.Generic;

namespace PUNDERO.Models;

public partial class Client
{
    public int IdClient { get; set; }

    public string NameObject { get; set; } = null!;

    public int? IdAccount { get; set; }

    public virtual Account? IdAccountNavigation { get; set; }

    public virtual ICollection<Object> Objects { get; set; } = new List<Object>();
}
