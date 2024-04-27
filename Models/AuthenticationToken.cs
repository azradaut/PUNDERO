using System;
using System.Collections.Generic;

namespace PUNDERO.Models;

public partial class AuthenticationToken
{
    public int IdAuthentication { get; set; }

    public string? TokenValue { get; set; }

    public int? IdAccount { get; set; }

    public string? Email { get; set; }

    public DateTime? SignDate { get; set; }

    public virtual Account? IdAccountNavigation { get; set; }
}
