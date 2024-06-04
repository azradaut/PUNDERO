public class DriverViewModel
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string LicenseNumber { get; set; }
    public string LicenseCategory { get; set; }
    public string TachographLabel { get; set; }
    public DateTime TachographIssueDate { get; set; }
    public DateTime TachographExpiryDate { get; set; }
    public string Image { get; set; } // Optional field
}
