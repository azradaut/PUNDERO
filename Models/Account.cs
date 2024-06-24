using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PUNDERO.Models;

public partial class Account
{
    public int IdAccount { get; set; }
    [Required(ErrorMessage = "Name is a required field")]
    [StringLength(50, ErrorMessage = "First name can have up to 50 characters")]
    public string FirstName { get; set; } = null!;
    [Required(ErrorMessage = "Last name is a required field")]
    [StringLength(50, ErrorMessage = "Last name can have up to 50 characters")]
    public string LastName { get; set; } = null!;
    [Required(ErrorMessage = "Email is a required field")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [StringLength(50, ErrorMessage = "Email can have up to 50 characters")]
    public string Email { get; set; } = null!;
    [Required(ErrorMessage = "Password is a required field")]
    [StringLength(20, ErrorMessage = "Password can have up to 20 characters")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$", ErrorMessage = "Password must be at least 8 characters long and contain at least one uppercase letter, one lowercase letter, one digit, and one special character")]
    public string Password { get; set; } = null!;

    public int Type { get; set; }

    public string? Image { get; set; }

    public virtual ICollection<AuthenticationToken> AuthenticationTokens { get; set; } = new List<AuthenticationToken>();

    public virtual ICollection<Client> Clients { get; set; } = new List<Client>();

    public virtual ICollection<Coordinator> Coordinators { get; set; } = new List<Coordinator>();

    public virtual ICollection<Driver> Drivers { get; set; } = new List<Driver>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}
