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
                .Select(md => new MobileDriverViewModel
                {
                    IdMobileDriver = md.IdMobileDriver,
                    DriverName = md.IdDriverNavigation != null ? md.IdDriverNavigation.IdAccountNavigation.FirstName + " " + md.IdDriverNavigation.IdAccountNavigation.LastName : "Unassigned",
                    MobilePhoneNumber = md.IdMobileNavigation.PhoneNumber,
                    AssignmentStartDate = md.AssignmentStartDate,
                    AssignmentEndDate = md.AssignmentEndDate.HasValue && md.AssignmentEndDate.Value == new DateTime(9999, 12, 31) ? null : md.AssignmentEndDate,
                    AssignmentType = md.IdAssignmentTypeNavigation.Description,
                    Note = md.Note
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
                .Select(md => new MobileDriverViewModel
                {
                    IdMobileDriver = md.IdMobileDriver,
                    DriverName = md.IdDriverNavigation != null ? md.IdDriverNavigation.IdAccountNavigation.FirstName + " " + md.IdDriverNavigation.IdAccountNavigation.LastName : "Unassigned",
                    MobilePhoneNumber = md.IdMobileNavigation.PhoneNumber,
                    AssignmentStartDate = md.AssignmentStartDate,
                    AssignmentEndDate = md.AssignmentEndDate.HasValue && md.AssignmentEndDate.Value == new DateTime(9999, 12, 31) ? null : md.AssignmentEndDate,
                    AssignmentType = md.IdAssignmentTypeNavigation.Description,
                    Note = md.Note
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
        public async Task<IActionResult> AddMobileAssignment([FromBody] MobileDriverViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var driver = await _context.Drivers
                .Include(d => d.IdAccountNavigation)
                .FirstOrDefaultAsync(d => d.IdAccountNavigation.FirstName + " " + d.IdAccountNavigation.LastName == viewModel.DriverName);

            if (driver == null)
            {
                return BadRequest("Driver not found.");
            }

            var mobile = await _context.Mobiles
                .FirstOrDefaultAsync(m => m.PhoneNumber == viewModel.MobilePhoneNumber);

            if (mobile == null)
            {
                return BadRequest("Mobile not found.");
            }

            var assignmentType = await _context.AssignmentTypes
                .FirstOrDefaultAsync(at => at.Description == viewModel.AssignmentType);

            if (assignmentType == null)
            {
                return BadRequest("Assignment type not found.");
            }

            var existingAssignment = await _context.MobileDrivers
                .Where(md => md.IdDriver == driver.IdDriver && md.IdAssignmentType == 1)
                .AnyAsync();

            if (assignmentType.IdAssignmentType == 1 && existingAssignment)
            {
                return BadRequest("Driver already has a permanent mobile assignment.");
            }

            var mobileDriver = new MobileDriver
            {
                IdDriver = driver.IdDriver,
                IdMobile = mobile.IdMobile,
                IdAssignmentType = assignmentType.IdAssignmentType,
                AssignmentStartDate = viewModel.AssignmentStartDate,
                AssignmentEndDate = assignmentType.IdAssignmentType == 1 ? new DateTime(9999, 12, 31) : viewModel.AssignmentEndDate,
                Note = viewModel.Note
            };

            _context.MobileDrivers.Add(mobileDriver);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMobileAssignment", new { id = mobileDriver.IdMobileDriver }, mobileDriver);
        }

        // PUT: api/MobileDriver/EditMobileAssignment/{id}]
        [HttpPut("EditMobileAssignment/{id}")]
        public async Task<IActionResult> EditMobileAssignment(int id, [FromBody] MobileDriverViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingAssignment = await _context.MobileDrivers.FindAsync(id);
            if (existingAssignment == null)
            {
                return NotFound("Assignment not found.");
            }

            var driver = await _context.Drivers
                .Include(d => d.IdAccountNavigation)
                .FirstOrDefaultAsync(d => d.IdAccountNavigation.FirstName + " " + d.IdAccountNavigation.LastName == viewModel.DriverName);

            if (driver == null)
            {
                return BadRequest("Driver not found.");
            }

            var mobile = await _context.Mobiles
                .FirstOrDefaultAsync(m => m.PhoneNumber == viewModel.MobilePhoneNumber);

            if (mobile == null)
            {
                return BadRequest("Mobile not found.");
            }

            var assignmentType = await _context.AssignmentTypes
                .FirstOrDefaultAsync(at => at.Description == viewModel.AssignmentType);

            if (assignmentType == null)
            {
                return BadRequest("Assignment type not found.");
            }

            existingAssignment.IdDriver = driver.IdDriver;
            existingAssignment.IdMobile = mobile.IdMobile;
            existingAssignment.IdAssignmentType = assignmentType.IdAssignmentType;
            existingAssignment.AssignmentStartDate = viewModel.AssignmentStartDate;
            existingAssignment.AssignmentEndDate = assignmentType.IdAssignmentType == 1 ? new DateTime(9999, 12, 31) : viewModel.AssignmentEndDate;
            existingAssignment.Note = viewModel.Note;

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

        [HttpGet("GetDriverAndAssignmentType/{phoneNumber}")]
        public async Task<IActionResult> GetDriverAndAssignmentType(int phoneNumber)
        {
            var assignment = await _context.MobileDrivers
                .Include(md => md.IdDriverNavigation)
                    .ThenInclude(d => d.IdAccountNavigation)
                .Include(md => md.IdAssignmentTypeNavigation)
                .Include(md => md.IdMobileNavigation) 
                .Where(md => md.IdMobileNavigation.PhoneNumber == phoneNumber)
                .Select(md => new
                {
                    DriverName = md.IdDriverNavigation != null ? md.IdDriverNavigation.IdAccountNavigation.FirstName + " " + md.IdDriverNavigation.IdAccountNavigation.LastName : "No Driver",
                    AssignmentType = md.IdAssignmentTypeNavigation.Description
                })
                .FirstOrDefaultAsync();

            if (assignment == null)
            {
                return NotFound("No assignment found for the given phone number.");
            }

            return Ok(assignment);
        }

    }
}