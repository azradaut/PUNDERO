using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PUNDERO.Models;

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
            var vehicles = _context.Vehicles.ToList();
            return Ok(vehicles);
        }

        // GET: api/Vehicles/registration
        [HttpGet("{registration}")]
        public IActionResult GetVehicleByRegistration(string registration)
        {
            var vehicle = _context.Vehicles.FirstOrDefault(v => v.Registration == registration);

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

            return CreatedAtRoute("GetVehicleByRegistration", new { registration = vehicle.Registration }, vehicle);
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
        [HttpDelete("{id}")]
        public IActionResult DeleteVehicle(int id)
        {
            var vehicle = _context.Vehicles.FirstOrDefault(v => v.IdVehicle == id);
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
