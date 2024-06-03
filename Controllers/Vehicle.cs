using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PUNDERO.Models;
using System.Linq;

namespace PUNDERO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehicleController : ControllerBase
    {
        private readonly PunderoContext _context;

        public VehicleController(PunderoContext context)
        {
            _context = context;
        }

        // GET: api/Vehicles
        [HttpGet]
        public IActionResult GetVehicles()
        {
            var vehicles = _context.Vehicles
                .Include(v => v.VehicleDrivers)
                    .ThenInclude(vd => vd.IdDriverNavigation)
                        .ThenInclude(d => d.IdAccountNavigation)
                .Include(v => v.VehicleDrivers)
                    .ThenInclude(vd => vd.IdAssignmentTypeNavigation)
                .Select(v => new
                {
                    v.IdVehicle,
                    v.Registration,
                    v.IssueDate,
                    v.ExpiryDate,
                    v.Brand,
                    v.Model,
                    v.Color,
                    AssignedDriver = v.VehicleDrivers.Select(vd => new
                    {
                        IdDriver = vd.IdDriver ?? 0,
                        DriverName = vd.IdDriverNavigation != null
                            ? vd.IdDriverNavigation.IdAccountNavigation.FirstName + " " + vd.IdDriverNavigation.IdAccountNavigation.LastName
                            : "Unassigned"
                    }).FirstOrDefault() ?? new { IdDriver = 0, DriverName = "Unassigned" },
                    AssignmentType = v.VehicleDrivers.Select(vd => vd.IdAssignmentTypeNavigation != null
                        ? vd.IdAssignmentTypeNavigation.Description
                        : "Unassigned").FirstOrDefault() ?? "Unassigned"
                })
                .ToList();

            return Ok(vehicles);
        }

        [HttpGet("GetUnassignedVehicles")]
        public async Task<IActionResult> GetUnassignedVehicles()
        {
            var vehicles = await _context.Vehicles
                .Where(v => !_context.VehicleDrivers.Any(vd => vd.IdVehicle == v.IdVehicle))
                .Select(v => new { v.IdVehicle, v.Registration })
                .ToListAsync();
            return Ok(vehicles);
        }

        // GET: api/Vehicles/registration
        [HttpGet("{registration}")]
        public IActionResult GetVehicleByRegistration(string registration)
        {
            var vehicle = _context.Vehicles
                .Include(v => v.VehicleDrivers)
                    .ThenInclude(vd => vd.IdDriverNavigation)
                        .ThenInclude(d => d.IdAccountNavigation)
                .Include(v => v.VehicleDrivers)
                    .ThenInclude(vd => vd.IdAssignmentTypeNavigation)
                .Where(v => v.Registration == registration)
                .Select(v => new
                {
                    v.IdVehicle,
                    v.Registration,
                    v.IssueDate,
                    v.ExpiryDate,
                    v.Brand,
                    v.Model,
                    v.Color,
                    AssignedDriver = v.VehicleDrivers.Select(vd => new
                    {
                        IdDriver = vd.IdDriver ?? 0,
                        DriverName = vd.IdDriverNavigation != null
                            ? vd.IdDriverNavigation.IdAccountNavigation.FirstName + " " + vd.IdDriverNavigation.IdAccountNavigation.LastName
                            : "Unassigned"
                    }).FirstOrDefault() ?? new { IdDriver = 0, DriverName = "Unassigned" },
                    AssignmentType = v.VehicleDrivers.Select(vd => vd.IdAssignmentTypeNavigation != null
                        ? vd.IdAssignmentTypeNavigation.Description
                        : "Unassigned").FirstOrDefault() ?? "Unassigned"
                })
                .FirstOrDefault();

            if (vehicle == null)
            {
                return NotFound();
            }

            return Ok(vehicle);
        }


        // POST: api/Vehicles
        [HttpPost]
        public IActionResult PostVehicle([FromBody] Vehicle vehicle)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Vehicles.Add(vehicle);
            _context.SaveChanges();

            return CreatedAtAction("GetVehicleByRegistration", new { registration = vehicle.Registration }, vehicle);
        }

        // PUT: api/Vehicles/id
        [HttpPut("{id}")]
        public IActionResult PutVehicle(int id, [FromBody] Vehicle vehicle)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != vehicle.IdVehicle)
            {
                return BadRequest();
            }

            _context.Entry(vehicle).State = EntityState.Modified;
            _context.SaveChanges();

            return NoContent();
        }



        // DELETE: api/Vehicles/id
        [HttpDelete("DeleteVehicle/{id}")]
        public IActionResult DeleteVehicle(int id)
        {
            var vehicle = _context.Vehicles.FirstOrDefault(i => i.IdVehicle == id);
            if (vehicle == null)
            {
                return NotFound();
            }

            _context.Vehicles.Remove(vehicle);
            _context.SaveChanges();

            return Ok(vehicle);
        }
    }
}