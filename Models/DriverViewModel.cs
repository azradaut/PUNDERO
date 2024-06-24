using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

public class DriverViewModel
{
    public int? IdDriver { get; set; }
    public int? IdAccount { get; set; }

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

    [StringLength(20, ErrorMessage = "License number can have up to 20 characters")]
    public string? LicenseNumber { get; set; }

    [StringLength(20, ErrorMessage = "License category can have up to 20 characters")]
    public string? LicenseCategory { get; set; }

    public string? TachographLabel { get; set; }

    public DateTime? TachographIssueDate { get; set; }

    public DateTime? TachographExpiryDate { get; set; }

    public string? Image { get; set; }
    public IFormFile? ImageFile { get; set; }
}
