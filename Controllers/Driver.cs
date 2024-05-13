using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PUNDERO.Models;

namespace PUNDERO.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class DriverController : ControllerBase
    {
        private readonly PunderoContext _context;

        public DriverController(PunderoContext context)
        {
            _context = context;
        }

        // GET: api/Driver
        [HttpGet]
        public IActionResult GetDrivers()
        {
            var drivers = _context.Drivers.ToList();
            return Ok(drivers);
        }
        // GET: api/Driver/IdAccount
        [HttpGet("{IdAccount}")]
        public IActionResult GetDriverByIdAccount(int IdAccount)
        {
            var driver = _context.Drivers.FirstOrDefault(v => v.IdAccount == IdAccount);

            if (driver == null)
            {
                return NotFound();
            }

            return Ok(driver);
        }

        // GET: api/Driver/IdDriver
        [HttpGet("{id}")]
        public IActionResult GetDriver(int id)
        {
            var driver = _context.Drivers.FirstOrDefault(v => v.IdDriver == id);

            if (driver == null)
            {
                return NotFound();
            }

            return Ok(driver);
        }
        // POST: api/Driver
        [HttpPost]
        public IActionResult PostDriver([FromBody] Driver driver)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Drivers.Add(driver);
            _context.SaveChanges();

            return CreatedAtRoute("GetDriver", new { id = driver.IdDriver }, driver);
        }

        // PUT: api/Driver/1
        [HttpPut("{id}")]
        public IActionResult PutDriver(int id, [FromBody] Driver driver)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != driver.IdDriver)
            {
                return BadRequest();
            }

            _context.Entry(driver).State = EntityState.Modified;
            _context.SaveChanges();

            return NoContent();
        }

        // DELETE: api/Driver/1
        [HttpDelete("{id}")]
        public IActionResult DeleteDriver(int id)
        {
            var driver = _context.Drivers.FirstOrDefault(d => d.IdDriver == id);
            if (driver == null)
            {
                return NotFound();
            }

            _context.Drivers.Remove(driver);
            _context.SaveChanges();

            return Ok(driver);
        }
    }
}
