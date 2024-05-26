using System;
using System.Collections.Generic;

namespace PUNDERO.Models;

public partial class Account
{
    public int IdAccount { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int Type { get; set; }

    public string? Image { get; set; }

    public virtual ICollection<AuthenticationToken> AuthenticationTokens { get; set; } = new List<AuthenticationToken>();

    public virtual ICollection<Client> Clients { get; set; } = new List<Client>();

    public virtual ICollection<Coordinator> Coordinators { get; set; } = new List<Coordinator>();

    public virtual ICollection<Driver> Drivers { get; set; } = new List<Driver>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}
