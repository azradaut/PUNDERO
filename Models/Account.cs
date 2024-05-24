using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PUNDERO.Models
{
    public partial class Account
    {
        public int IdAccount { get; set; }

        [Required(ErrorMessage = "First Name is required")]
        [StringLength(50, ErrorMessage = "First Name cannot be longer than 50 characters")]
        public string FirstName { get; set; } = null!;

        [Required(ErrorMessage = "Last Name is required")]
        [StringLength(50, ErrorMessage = "Last Name cannot be longer than 50 characters")]
        public string LastName { get; set; } = null!;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [StringLength(50, ErrorMessage = "Email cannot be longer than 50 characters")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Password is required")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,20}$",
            ErrorMessage = "Password must be between 8 and 20 characters and contain at least one lowercase letter, one uppercase letter, one digit, and one special character")]
        [StringLength(20, ErrorMessage = "Password cannot be longer than 20 characters")]
        public string Password { get; set; } = null!;

        [Range(0, int.MaxValue, ErrorMessage = "Invalid Type value")]
        public int Type { get; set; }

        [StringLength(255, ErrorMessage = "Image path cannot be longer than 255 characters")]
        public string? Image { get; set; }

        public virtual ICollection<AuthenticationToken> AuthenticationTokens { get; set; } = new List<AuthenticationToken>();

        public virtual ICollection<Client> Clients { get; set; } = new List<Client>();

        public virtual ICollection<Coordinator> Coordinators { get; set; } = new List<Coordinator>();

        public virtual ICollection<Driver> Drivers { get; set; } = new List<Driver>();

        public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }
}
