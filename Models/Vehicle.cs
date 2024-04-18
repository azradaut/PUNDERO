using System;
using System.Collections.Generic;

namespace PUNDERO.Models;

public partial class Vehicle
{
    public int IdVehicle { get; set; }

    public string Registration { get; set; } = null!;

    public DateTime IssueDate { get; set; }

    public DateTime ExpiryDate { get; set; }

    public string Brand { get; set; } = null!;

    public string Model { get; set; } = null!;

    public string Color { get; set; } = null!;

    public int IdDriver { get; set; }

    public virtual ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();

    public virtual Driver IdDriverNavigation { get; set; } = null!;
}
