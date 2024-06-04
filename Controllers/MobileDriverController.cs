using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PUNDERO.Models;
using System.Linq;
using System.Threading.Tasks;

namespace PUNDERO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MobileDriverController : ControllerBase
    {
        private readonly PunderoContext _context;

        public MobileDriverController(PunderoContext context)
        {
            _context = context;
        }

        // GET: api/MobileDriver/GetMobileAssignments
        [HttpGet("GetMobileAssignments")]
        public async Task<IActionResult> GetMobileAssignments()
        {
            var assignments = await _context.MobileDrivers
                .Include(md => md.IdDriverNavigation)
                    .ThenInclude(d => d.IdAccountNavigation)
                .Include(md => md.IdMobileNavigation)
                .Include(md => md.IdAssignmentTypeNavigation)
                .Select(md => new
                {
                    md.IdMobileDriver,
                    DriverName = md.IdDriverNavigation.IdAccountNavigation.FirstName + " " + md.IdDriverNavigation.IdAccountNavigation.LastName,
                    PhoneNumber = md.IdMobileNavigation.PhoneNumber,
                    md.AssignmentStartDate,
                    md.AssignmentEndDate,
                    AssignmentType = md.IdAssignmentTypeNavigation.Description
                })
                .ToListAsync();

            return Ok(assignments);
        }

        // GET: api/MobileDriver/GetMobileAssignment/{id}
        [HttpGet("GetMobileAssignment/{id}")]
        public async Task<IActionResult> GetMobileAssignment(int id)
        {
            var assignment = await _context.MobileDrivers
                .Include(md => md.IdDriverNavigation)
                    .ThenInclude(d => d.IdAccountNavigation)
                .Include(md => md.IdMobileNavigation)
                .Include(md => md.IdAssignmentTypeNavigation)
                .Where(md => md.IdMobileDriver == id)
                .Select(md => new
                {
                    md.IdMobileDriver,
                    md.AssignmentStartDate,
                    md.AssignmentEndDate,
                    md.IdDriver,
                    md.IdMobile,
                    md.IdAssignmentType,
                    md.Note,
                    DriverName = md.IdDriverNavigation != null
                        ? md.IdDriverNavigation.IdAccountNavigation.FirstName + " " + md.IdDriverNavigation.IdAccountNavigation.LastName
                        : "Unassigned",
                    PhoneNumber = md.IdMobileNavigation != null
                        ? md.IdMobileNavigation.PhoneNumber
                        : 0,
                    AssignmentType = md.IdAssignmentTypeNavigation != null
                        ? md.IdAssignmentTypeNavigation.Description
                        : "Unassigned"
                })
                .FirstOrDefaultAsync();

            if (assignment == null)
            {
                return NotFound();
            }

            return Ok(assignment);
        }

        // POST: api/MobileDriver/AddMobileAssignment
        [HttpPost("AddMobileAssignment")]
        public async Task<IActionResult> AddMobileAssignment([FromBody] MobileDriver mobileDriver)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingAssignment = await _context.MobileDrivers
                .Where(md => md.IdDriver == mobileDriver.IdDriver && md.IdAssignmentType == 1)
                .AnyAsync();

            if (mobileDriver.IdAssignmentType == 1 && existingAssignment)
            {
                return BadRequest("Driver already has a permanent mobile assignment.");
            }

            if (mobileDriver.IdAssignmentType == 1 && !mobileDriver.AssignmentEndDate.HasValue)
            {
                mobileDriver.AssignmentEndDate = new DateTime(1, 1, 1); //0000-00-00 in DateTime
            }

            _context.MobileDrivers.Add(mobileDriver);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMobileAssignment", new { id = mobileDriver.IdMobileDriver }, mobileDriver);
        }


        // PUT: api/MobileDriver/EditMobileAssignment/{id}
        [HttpPut("EditMobileAssignment/{id}")]
        public async Task<IActionResult> EditMobileAssignment(int id, [FromBody] MobileDriver mobileDriver)
        {
            if (id != mobileDriver.IdMobileDriver)
            {
                return BadRequest("Assignment ID mismatch.");
            }

            var existingAssignment = await _context.MobileDrivers.FindAsync(id);
            if (existingAssignment == null)
            {
                return NotFound("Assignment not found.");
            }

            existingAssignment.IdDriver = mobileDriver.IdDriver;
            existingAssignment.IdMobile = mobileDriver.IdMobile;
            existingAssignment.IdAssignmentType = mobileDriver.IdAssignmentType;
            existingAssignment.AssignmentStartDate = mobileDriver.AssignmentStartDate;
            existingAssignment.AssignmentEndDate = mobileDriver.AssignmentEndDate ?? new DateTime(1, 1, 1); // Default to 0000-00-00 if null
            existingAssignment.Note = mobileDriver.Note;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/MobileDriver/DeleteMobileAssignment/{id}
        [HttpDelete("DeleteMobileAssignment/{id}")]
        public async Task<IActionResult> DeleteMobileAssignment(int id)
        {
            var mobileDriver = await _context.MobileDrivers.FindAsync(id);
            if (mobileDriver == null)
            {
                return NotFound("Assignment not found.");
            }

            _context.MobileDrivers.Remove(mobileDriver);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // GET: api/MobileDriver/GetUnassignedMobiles
        [HttpGet("GetUnassignedMobiles")]
        public async Task<IActionResult> GetUnassignedMobiles()
        {
            var mobiles = await _context.Mobiles
                .Where(m => !m.MobileDrivers.Any())
                .Select(m => new
                {
                    m.IdMobile,
                    m.PhoneNumber
                })
                .ToListAsync();

            return Ok(mobiles);
        }

        // GET: api/MobileDriver/GetAssignmentTypes
        [HttpGet("GetAssignmentTypes")]
        public async Task<IActionResult> GetAssignmentTypes()
        {
            var assignmentTypes = await _context.AssignmentTypes
                .Select(at => new
                {
                    at.IdAssignmentType,
                    at.Description
                })
                .ToListAsync();

            return Ok(assignmentTypes);
        }

        // GET: api/MobileDriver/GetDriversWithName
        [HttpGet("GetDriversWithName")]
        public async Task<IActionResult> GetDriversWithName()
        {
            var drivers = await _context.Drivers
                .Include(d => d.IdAccountNavigation)
                .Select(d => new
                {
                    d.IdDriver,
                    FullName = d.IdAccountNavigation.FirstName + " " + d.IdAccountNavigation.LastName,
                    HasPermanentMobile = d.MobileDrivers.Any(md => md.IdAssignmentType == 1)
                })
                .ToListAsync();

            return Ok(drivers);
        }
    }
}
