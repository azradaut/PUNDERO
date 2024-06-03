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

        [HttpGet("GetAssignments")]
        public async Task<IActionResult> GetAssignments()
        {
            var assignments = await _context.MobileDrivers
                .Include(md => md.IdDriverNavigation)
                .ThenInclude(d => d.IdAccountNavigation)
                .Include(md => md.IdMobileNavigation)
                .Include(md => md.IdAssignmentTypeNavigation)
                .Select(md => new
                {
                    md.IdMobileDriver,
                    DriverName = $"{md.IdDriverNavigation.IdAccountNavigation.FirstName} {md.IdDriverNavigation.IdAccountNavigation.LastName}",
                    MobilePhoneNumber = md.IdMobileNavigation.PhoneNumber,
                    md.AssignmentStartDate,
                    md.AssignmentEndDate,
                    AssignmentType = md.IdAssignmentTypeNavigation.Description
                })
                .ToListAsync();

            return Ok(assignments);
        }

        [HttpPost("AddAssignment")]
        public async Task<IActionResult> AddAssignment([FromBody] MobileDriverViewModel mobileDriverVM)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var drivers = await _context.Drivers
                .Include(d => d.IdAccountNavigation)
                .ToListAsync();

            var mobiles = await _context.Mobiles.ToListAsync();
            var assignmentTypes = await _context.AssignmentTypes.ToListAsync();

            var driver = drivers.FirstOrDefault(d => $"{d.IdAccountNavigation.FirstName} {d.IdAccountNavigation.LastName}" == mobileDriverVM.DriverName);
            var mobile = mobiles.FirstOrDefault(m => m.PhoneNumber == mobileDriverVM.MobilePhoneNumber);
            var assignmentType = assignmentTypes.FirstOrDefault(at => at.Description == mobileDriverVM.AssignmentType);

            if (driver == null || mobile == null || assignmentType == null)
            {
                return BadRequest("Invalid driver, mobile, or assignment type");
            }

            var mobileDriver = new MobileDriver
            {
                IdDriver = driver.IdDriver,
                IdMobile = mobile.IdMobile,
                IdAssignmentType = assignmentType.IdAssignmentType,
                AssignmentStartDate = mobileDriverVM.AssignmentStartDate,
                AssignmentEndDate = mobileDriverVM.AssignmentEndDate
            };

            _context.MobileDrivers.Add(mobileDriver);
            await _context.SaveChangesAsync();

            return Ok(mobileDriver);
        }

        [HttpPut("EditAssignment/{id}")]
        public async Task<IActionResult> EditAssignment(int id, [FromBody] MobileDriverViewModel mobileDriverVM)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var mobileDriver = await _context.MobileDrivers.FindAsync(id);
            if (mobileDriver == null)
            {
                return NotFound();
            }

            var drivers = await _context.Drivers
                .Include(d => d.IdAccountNavigation)
                .ToListAsync();

            var mobiles = await _context.Mobiles.ToListAsync();
            var assignmentTypes = await _context.AssignmentTypes.ToListAsync();

            var driver = drivers.FirstOrDefault(d => $"{d.IdAccountNavigation.FirstName} {d.IdAccountNavigation.LastName}" == mobileDriverVM.DriverName);
            var mobile = mobiles.FirstOrDefault(m => m.PhoneNumber == mobileDriverVM.MobilePhoneNumber);
            var assignmentType = assignmentTypes.FirstOrDefault(at => at.Description == mobileDriverVM.AssignmentType);

            if (driver == null || mobile == null || assignmentType == null)
            {
                return BadRequest("Invalid driver, mobile, or assignment type");
            }

            mobileDriver.IdDriver = driver.IdDriver;
            mobileDriver.IdMobile = mobile.IdMobile;
            mobileDriver.IdAssignmentType = assignmentType.IdAssignmentType;
            mobileDriver.AssignmentStartDate = mobileDriverVM.AssignmentStartDate;
            mobileDriver.AssignmentEndDate = mobileDriverVM.AssignmentEndDate;

            _context.Entry(mobileDriver).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("DeleteAssignment/{id}")]
        public async Task<IActionResult> DeleteAssignment(int id)
        {
            var mobileDriver = await _context.MobileDrivers.FindAsync(id);
            if (mobileDriver == null)
            {
                return NotFound();
            }

            _context.MobileDrivers.Remove(mobileDriver);
            await _context.SaveChangesAsync();

            return Ok(mobileDriver);
        }

        [HttpGet("GetUnassignedMobiles")]
        public async Task<IActionResult> GetUnassignedMobiles()
        {
            var unassignedMobiles = await _context.Mobiles
                .Where(m => !_context.MobileDrivers.Any(md => md.IdMobile == m.IdMobile && md.AssignmentEndDate > DateTime.Now))
                .ToListAsync();

            return Ok(unassignedMobiles);
        }

        [HttpGet("GetDriversWithName")]
        public async Task<IActionResult> GetDriversWithName()
        {
            var drivers = await _context.Drivers
                .Include(d => d.IdAccountNavigation)
                .Select(d => new
                {
                    d.IdDriver,
                    d.IdAccountNavigation.FirstName,
                    d.IdAccountNavigation.LastName
                })
                .ToListAsync();

            return Ok(drivers);
        }

        [HttpGet("GetAssignmentTypes")]
        public async Task<IActionResult> GetAssignmentTypes()
        {
            var assignmentTypes = await _context.AssignmentTypes.ToListAsync();
            return Ok(assignmentTypes);
        }
    }
}
