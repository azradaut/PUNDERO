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
    public class WarehousesController : ControllerBase
    {
        private readonly PunderoContext _context;

        public WarehousesController(PunderoContext context)
        {
            _context = context;
        }

        // GET: api/Warehouses
        [HttpGet]
        public IActionResult GetWarehouses()
        {
            var warehouses = _context.Warehouses.OrderByDescending(w => w.IdWarehouse).ToList();
            return Ok(warehouses);
        }

        // GET: api/Warehouses/5
        [HttpGet("{id}")]
        public IActionResult GetWarehouse(int id)
        {
            var warehouse = _context.Warehouses.FirstOrDefault(w => w.IdWarehouse == id);

            if (warehouse == null)
            {
                return NotFound();
            }

            return Ok(warehouse);
        }
        // GET: api/Warehouses/name
        [HttpGet("{name}")]
        public IActionResult GetWarehouse(string name)
        {
            var warehouse = _context.Warehouses.FirstOrDefault(w => w.NameWarehouse == name);

            if (warehouse == null)
            {
                return NotFound();
            }

            return Ok(warehouse);
        }

        // POST: api/Warehouses
        [HttpPost]
        public IActionResult PostWarehouse([FromBody] Warehouse warehouse)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Warehouses.Add(warehouse);
            _context.SaveChanges();

            return CreatedAtRoute("GetWarehouse", new { id = warehouse.IdWarehouse }, warehouse);
        }

        // PUT: api/Warehouses/5
        [HttpPut("{id}")]
        public IActionResult PutWarehouse(int id, [FromBody] Warehouse warehouse)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != warehouse.IdWarehouse)
            {
                return BadRequest();
            }

            _context.Entry(warehouse).State = EntityState.Modified;
            _context.SaveChanges();

            return NoContent();
        }

        // PUT: api/Warehouses/name
        [HttpPut("{name}")]
        public IActionResult PutWarehouse(string name, [FromBody] Warehouse warehouse)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if a warehouse with the provided name (but potentially different ID) exists
            var existingWarehouse = _context.Warehouses.FirstOrDefault(w => w.NameWarehouse == name && w.IdWarehouse != warehouse.IdWarehouse);

            if (existingWarehouse != null)
            {
                return BadRequest("Warehouse name already exists.");
            }

            // Update the warehouse based on the provided name (assuming it exists)
            _context.Entry(warehouse).State = EntityState.Modified;
            _context.SaveChanges();

            return NoContent();
        }


        // DELETE: api/Warehouses/5
        [HttpDelete("{id}")]
        public IActionResult DeleteWarehouse(int id)
        {
            var warehouse = _context.Warehouses.FirstOrDefault(w => w.IdWarehouse == id);
            if (warehouse == null)
            {
                return NotFound();
            }

            _context.Warehouses.Remove(warehouse);
            _context.SaveChanges();

            return Ok(warehouse);
        }
        
        // DELETE: api/Warehouses/name
        [HttpDelete("{name}")]
        public IActionResult DeleteWarehouse(string name)
        {
            var warehouse = _context.Warehouses.FirstOrDefault(w => w.NameWarehouse == name);
            if (warehouse == null)
            {
                return NotFound();
            }

            _context.Warehouses.Remove(warehouse);
            _context.SaveChanges();

            return Ok(warehouse);
        }
    }
}
