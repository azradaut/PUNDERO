using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PUNDERO.Models;

public partial class Vehicle
{
    public int IdVehicle { get; set; }

    //[Required(ErrorMessage = "Registration is a required field")]
    //[RegularExpression(@"^[A-Z]{1}\d{2}-[A-Z]{1}-\d{3}$", ErrorMessage = "Vehicle registration must follow the format 'A11-B-222'")]
    public string Registration { get; set; } = null!;

    //[Required(ErrorMessage = "Issue date is a required field")]
    public DateTime IssueDate { get; set; }

    //[Required(ErrorMessage = "Expiry date is a required field")]
    public DateTime ExpiryDate { get; set; }

    //[Required(ErrorMessage = "Brand is a required field")]
    //[StringLength(50, ErrorMessage = "Brand can have up to 50 characters")]
    public string Brand { get; set; } = null!;

    //[Required(ErrorMessage = "Model is a required field")]
    //[StringLength(50, ErrorMessage = "Model can have up to 50 characters")]
    public string Model { get; set; } = null!;

    //[Required(ErrorMessage = "Color is a required field")]
    //[StringLength(20, ErrorMessage = "Color can have up to 20 characters")]
    public string Color { get; set; } = null!;

    public virtual ICollection<VehicleDriver> VehicleDrivers { get; set; } = new List<VehicleDriver>();
}
