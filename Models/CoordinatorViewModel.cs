using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

public class CoordinatorViewModel
{
    public int IdCoordinator { get; set; }
    public int IdAccount { get; set; }

    [Required(ErrorMessage = "First name is a required field")]
    [StringLength(50, ErrorMessage = "First name can have up to 50 characters")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "Last name is a required field")]
    [StringLength(50, ErrorMessage = "Last name can have up to 50 characters")]
    public string LastName { get; set; }

    [Required(ErrorMessage = "Email is a required field")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [StringLength(50, ErrorMessage = "Email can have up to 50 characters")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Password is a required field")]
    [StringLength(20, ErrorMessage = "Password can have up to 20 characters")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$", ErrorMessage = "Password must be at least 8 characters long and contain at least one uppercase letter, one lowercase letter, one digit, and one special character")]
    public string Password { get; set; }

    [StringLength(50, ErrorMessage = "Qualification can have up to 50 characters")]
    public string Qualification { get; set; }

    [StringLength(200, ErrorMessage = "Description can have up to 200 characters")]
    public string Description { get; set; }

    public string? Image { get; set; }
    public IFormFile? ImageFile { get; set; }
}
