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

        // GET: api/VehicleDriver/GetVehicleAssignments
        [HttpGet("GetVehicleAssignments")]
        public async Task<IActionResult> GetVehicleAssignments()
        {
            var assignments = await _context.VehicleDrivers
                .Include(vd => vd.IdDriverNavigation)
                    .ThenInclude(d => d.IdAccountNavigation)
                .Include(vd => vd.IdVehicleNavigation)
                .Include(vd => vd.IdAssignmentTypeNavigation)
                .Select(vd => new VehicleDriverViewModel
                {
                    IdVehicleDriver = vd.IdVehicleDriver,
                    DriverName = vd.IdDriverNavigation != null ? vd.IdDriverNavigation.IdAccountNavigation.FirstName + " " + vd.IdDriverNavigation.IdAccountNavigation.LastName : "Unassigned",
                    VehicleRegistration = vd.IdVehicleNavigation.Registration,
                    AssignmentStartDate = vd.AssignmentStartDate,
                    AssignmentEndDate = vd.AssignmentEndDate.HasValue && vd.AssignmentEndDate.Value == new DateTime(9999, 12, 31) ? null : vd.AssignmentEndDate,
                    AssignmentType = vd.IdAssignmentTypeNavigation.Description,
                    Note = vd.Note
                })
                .ToListAsync();

            return Ok(assignments);
        }

        // GET: api/VehicleDriver/GetVehicleAssignment/{id}
        [HttpGet("GetVehicleAssignment/{id}")]
        public async Task<IActionResult> GetVehicleAssignment(int id)
        {
            var assignment = await _context.VehicleDrivers
                .Include(vd => vd.IdDriverNavigation)
                    .ThenInclude(d => d.IdAccountNavigation)
                .Include(vd => vd.IdVehicleNavigation)
                .Include(vd => vd.IdAssignmentTypeNavigation)
                .Where(vd => vd.IdVehicleDriver == id)
                .Select(vd => new VehicleDriverViewModel
                {
                    IdVehicleDriver = vd.IdVehicleDriver,
                    DriverName = vd.IdDriverNavigation != null ? vd.IdDriverNavigation.IdAccountNavigation.FirstName + " " + vd.IdDriverNavigation.IdAccountNavigation.LastName : "Unassigned",
                    VehicleRegistration = vd.IdVehicleNavigation.Registration,
                    AssignmentStartDate = vd.AssignmentStartDate,
                    AssignmentEndDate = vd.AssignmentEndDate.HasValue && vd.AssignmentEndDate.Value == new DateTime(9999, 12, 31) ? null : vd.AssignmentEndDate,
                    AssignmentType = vd.IdAssignmentTypeNavigation.Description,
                    Note = vd.Note
                })
                .FirstOrDefaultAsync();

            if (assignment == null)
            {
                return NotFound();
            }

            return Ok(assignment);
        }

        // POST: api/VehicleDriver/AddVehicleAssignment
        [HttpPost("AddVehicleAssignment")]
        public async Task<IActionResult> AddVehicleAssignment([FromBody] VehicleDriverViewModel viewModel)
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

            var vehicle = await _context.Vehicles
                .FirstOrDefaultAsync(v => v.Registration == viewModel.VehicleRegistration);

            if (vehicle == null)
            {
                return BadRequest("Vehicle not found.");
            }

            var assignmentType = await _context.AssignmentTypes
                .FirstOrDefaultAsync(at => at.Description == viewModel.AssignmentType);

            if (assignmentType == null)
            {
                return BadRequest("Assignment type not found.");
            }

            var existingAssignment = await _context.VehicleDrivers
                .Where(vd => vd.IdDriver == driver.IdDriver && vd.IdAssignmentType == 1)
                .AnyAsync();

            if (assignmentType.IdAssignmentType == 1 && existingAssignment)
            {
                return BadRequest("Driver already has a permanent vehicle assignment.");
            }

            var vehicleDriver = new VehicleDriver
            {
                IdDriver = driver.IdDriver,
                IdVehicle = vehicle.IdVehicle,
                IdAssignmentType = assignmentType.IdAssignmentType,
                AssignmentStartDate = viewModel.AssignmentStartDate,
                AssignmentEndDate = assignmentType.IdAssignmentType == 1 ? new DateTime(9999, 12, 31) : viewModel.AssignmentEndDate,
                Note = viewModel.Note
            };

            _context.VehicleDrivers.Add(vehicleDriver);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetVehicleAssignment", new { id = vehicleDriver.IdVehicleDriver }, vehicleDriver);
        }

        // PUT: api/VehicleDriver/EditVehicleAssignment/{id}]
        [HttpPut("EditVehicleAssignment/{id}")]
        public async Task<IActionResult> EditVehicleAssignment(int id, [FromBody] VehicleDriverViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingAssignment = await _context.VehicleDrivers.FindAsync(id);
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

            var vehicle = await _context.Vehicles
                .FirstOrDefaultAsync(v => v.Registration == viewModel.VehicleRegistration);

            if (vehicle == null)
            {
                return BadRequest("Vehicle not found.");
            }

            var assignmentType = await _context.AssignmentTypes
                .FirstOrDefaultAsync(at => at.Description == viewModel.AssignmentType);

            if (assignmentType == null)
            {
                return BadRequest("Assignment type not found.");
            }

            existingAssignment.IdDriver = driver.IdDriver;
            existingAssignment.IdVehicle = vehicle.IdVehicle;
            existingAssignment.IdAssignmentType = assignmentType.IdAssignmentType;
            existingAssignment.AssignmentStartDate = viewModel.AssignmentStartDate;
            existingAssignment.AssignmentEndDate = assignmentType.IdAssignmentType == 1 ? new DateTime(9999, 12, 31) : viewModel.AssignmentEndDate;
            existingAssignment.Note = viewModel.Note;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/VehicleDriver/DeleteVehicleAssignment/{id}
        [HttpDelete("DeleteVehicleAssignment/{id}")]
        public async Task<IActionResult> DeleteVehicleAssignment(int id)
        {
            var vehicleDriver = await _context.VehicleDrivers.FindAsync(id);
            if (vehicleDriver == null)
            {
                return NotFound("Assignment not found.");
            }

            _context.VehicleDrivers.Remove(vehicleDriver);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // GET: api/VehicleDriver/GetUnassignedVehicles
        [HttpGet("GetUnassignedVehicles")]
        public async Task<IActionResult> GetUnassignedVehicles()
        {
            var vehicles = await _context.Vehicles
                .Where(v => !v.VehicleDrivers.Any())
                .Select(v => new
                {
                    v.IdVehicle,
                    v.Registration
                })
                .ToListAsync();

            return Ok(vehicles);
        }

        // GET: api/VehicleDriver/GetAssignmentTypes
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

        // GET: api/VehicleDriver/GetDriversWithName
        [HttpGet("GetDriversWithName")]
        public async Task<IActionResult> GetDriversWithName()
        {
            var drivers = await _context.Drivers
                .Include(d => d.IdAccountNavigation)
                .Select(d => new
                {
                    d.IdDriver,
                    FullName = d.IdAccountNavigation.FirstName + " " + d.IdAccountNavigation.LastName,
                    HasPermanentVehicle = d.VehicleDrivers.Any(vd => vd.IdAssignmentType == 1)
                })
                .ToListAsync();

            return Ok(drivers);
        }

        [HttpGet("GetDriverAndAssignmentType/{registration}")]
        public async Task<IActionResult> GetDriverAndAssignmentType(string registration)
        {
            var assignment = await _context.VehicleDrivers
                .Include(vd => vd.IdDriverNavigation)
                    .ThenInclude(d => d.IdAccountNavigation)
                .Include(vd => vd.IdAssignmentTypeNavigation)
                .Include(vd => vd.IdVehicleNavigation)
                .Where(vd => vd.IdVehicleNavigation.Registration == registration)
                .Select(vd => new
                {
                    DriverName = vd.IdDriverNavigation != null ? vd.IdDriverNavigation.IdAccountNavigation.FirstName + " " + vd.IdDriverNavigation.IdAccountNavigation.LastName : "No Driver",
                    AssignmentType = vd.IdAssignmentTypeNavigation.Description
                })
                .FirstOrDefaultAsync();

            if (assignment == null)
            {
                return NotFound("No assignment found for the given registration.");
            }

            return Ok(assignment);
        }

    }
}
