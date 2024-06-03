using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PUNDERO.Models;
using System.Linq;
using System.Threading.Tasks;

namespace PUNDERO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehicleDriverController : ControllerBase
    {
        private readonly PunderoContext _context;

        public VehicleDriverController(PunderoContext context)
        {
            _context = context;
        }

        [HttpGet("GetAssignments")]
        public async Task<IActionResult> GetAssignments()
        {
            var assignments = await _context.VehicleDrivers
                .Include(vd => vd.IdDriverNavigation)
                .ThenInclude(d => d.IdAccountNavigation)
                .Include(vd => vd.IdVehicleNavigation)
                .Include(vd => vd.IdAssignmentTypeNavigation)
                .Select(vd => new {
                    vd.IdVehicleDriver,
                    DriverName = vd.IdDriverNavigation.IdAccountNavigation.FirstName + " " + vd.IdDriverNavigation.IdAccountNavigation.LastName,
                    VehicleRegistration = vd.IdVehicleNavigation.Registration,
                    vd.AssignmentStartDate,
                    vd.AssignmentEndDate,
                    AssignmentType = vd.IdAssignmentTypeNavigation.Description
                })
                .ToListAsync();
            return Ok(assignments);
        }

        [HttpPost("AddAssignment")]
        public async Task<IActionResult> AddAssignment([FromBody] VehicleDriverViewModel vehicleDriverVM)
        {
            var driver = await _context.Drivers.Include(d => d.IdAccountNavigation).FirstOrDefaultAsync(d => d.IdAccountNavigation.FirstName + " " + d.IdAccountNavigation.LastName == vehicleDriverVM.DriverName);
            var vehicle = await _context.Vehicles.FirstOrDefaultAsync(v => v.Registration == vehicleDriverVM.VehicleRegistration);
            var assignmentType = await _context.AssignmentTypes.FirstOrDefaultAsync(at => at.Description == vehicleDriverVM.AssignmentType);

            if (driver == null || vehicle == null || assignmentType == null)
            {
                return BadRequest("Invalid driver, vehicle, or assignment type.");
            }

            var vehicleDriver = new VehicleDriver
            {
                IdDriver = driver.IdDriver,
                IdVehicle = vehicle.IdVehicle,
                AssignmentStartDate = vehicleDriverVM.AssignmentStartDate,
                AssignmentEndDate = vehicleDriverVM.AssignmentEndDate,
                IdAssignmentType = assignmentType.IdAssignmentType
            };

            _context.VehicleDrivers.Add(vehicleDriver);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("EditAssignment/{id}")]
        public async Task<IActionResult> EditAssignment(int id, [FromBody] VehicleDriverViewModel vehicleDriverVM)
        {
            var existingAssignment = await _context.VehicleDrivers.FindAsync(id);
            if (existingAssignment == null)
            {
                return NotFound();
            }

            var driver = await _context.Drivers.Include(d => d.IdAccountNavigation).FirstOrDefaultAsync(d => d.IdAccountNavigation.FirstName + " " + d.IdAccountNavigation.LastName == vehicleDriverVM.DriverName);
            var vehicle = await _context.Vehicles.FirstOrDefaultAsync(v => v.Registration == vehicleDriverVM.VehicleRegistration);
            var assignmentType = await _context.AssignmentTypes.FirstOrDefaultAsync(at => at.Description == vehicleDriverVM.AssignmentType);

            if (driver == null || vehicle == null || assignmentType == null)
            {
                return BadRequest("Invalid driver, vehicle, or assignment type.");
            }

            existingAssignment.IdDriver = driver.IdDriver;
            existingAssignment.IdVehicle = vehicle.IdVehicle;
            existingAssignment.AssignmentStartDate = vehicleDriverVM.AssignmentStartDate;
            existingAssignment.AssignmentEndDate = vehicleDriverVM.AssignmentEndDate;
            existingAssignment.IdAssignmentType = assignmentType.IdAssignmentType;

            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("DeleteAssignment/{id}")]
        public async Task<IActionResult> DeleteAssignment(int id)
        {
            var existingAssignment = await _context.VehicleDrivers.FindAsync(id);
            if (existingAssignment == null)
            {
                return NotFound();
            }

            _context.VehicleDrivers.Remove(existingAssignment);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
