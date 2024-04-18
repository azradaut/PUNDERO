using System;
using System.Collections.Generic;

namespace PUNDERO.Models;

public partial class AssignmentType
{
    public int IdAssignmentType { get; set; }

    public string Description { get; set; } = null!;

    public virtual ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();
}
