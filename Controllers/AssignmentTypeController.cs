using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PUNDERO.Models;
using System.Linq;
using System.Threading.Tasks;

namespace PUNDERO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssignmentTypeController : ControllerBase
    {
        private readonly PunderoContext _context;

        public AssignmentTypeController(PunderoContext context)
        {
            _context = context;
        }

        [HttpGet("GetAssignmentTypes")]
        public async Task<IActionResult> GetAssignmentTypes()
        {
            var assignmentTypes = await _context.AssignmentTypes
                .Select(at => new { at.IdAssignmentType, at.Description })
                .ToListAsync();
            return Ok(assignmentTypes);
        }
    }
}
