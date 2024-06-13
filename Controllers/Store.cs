using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PUNDERO.Models;

namespace PUNDERO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoresController : ControllerBase
    {
        private readonly PunderoContext _context;
        private readonly ILogger<StoresController> _logger;

        public StoresController(PunderoContext context, ILogger<StoresController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("GetStores")]
        public async Task<IActionResult> GetStoresByName()
        {
            var stores = await _context.Stores.Select(s => new { s.IdStore, s.Name }).ToListAsync();
            return Ok(stores);
        }

        // GET: api/Stores
        [HttpGet]
        public IActionResult GetStores()
        {
            var stores = _context.Stores
                .Include(s => s.IdClientNavigation)
                    .ThenInclude(c => c.IdAccountNavigation)
                .Select(s => new
                {
                    s.IdStore,
                    s.Name,
                    s.Address,
                    s.Longitude,
                    s.Latitude,
                    ClientName = s.IdClientNavigation != null
                        ? s.IdClientNavigation.IdAccountNavigation.FirstName + " " + s.IdClientNavigation.IdAccountNavigation.LastName
                        : "Unassigned client"
                })
                .ToList();

            return Ok(stores);
        }

        // GET: api/Stores/5
        [HttpGet("{id}")]
        public IActionResult GetStore(int id)
        {
            var store = _context.Stores
                .Include(s => s.IdClientNavigation)
                    .ThenInclude(c => c.IdAccountNavigation)
                .Where(s => s.IdStore == id)
                .Select(s => new
                {
                    s.IdStore,
                    s.Name,
                    s.Address,
                    s.Longitude,
                    s.Latitude,
                    ClientName = s.IdClientNavigation != null
                        ? s.IdClientNavigation.IdAccountNavigation.FirstName + " " + s.IdClientNavigation.IdAccountNavigation.LastName
                        : "Unassigned client"
                })
                .FirstOrDefault();

            if (store == null)
            {
                return NotFound();
            }

            return Ok(store);
        }

        [HttpGet("getStoreByName/{storeName}")]
        public async Task<IActionResult> GetStoreByName(string storeName)
        {
            try
            {
                var store = await _context.Stores
                    .Where(s => s.Name == storeName)
                    .Select(s => new {
                        IdStore = s.IdStore,
                        Name = s.Name,
                        Address = s.Address,
                        Location = new
                        {
                            Latitude = s.Latitude,
                            Longitude = s.Longitude
                        }
                    })
                    .FirstOrDefaultAsync();

                if (store == null)
                {
                    return NotFound("Store not found");
                }

                return Ok(store);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching the store details.");
                return StatusCode(500, "Internal server error");
            }
        }

        // POST: api/Stores
        [HttpPost]
        public async Task<IActionResult> AddStore([FromBody] Store model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var store = new Store
            {
                Name = model.Name,
                Address = model.Address,
                Longitude = model.Longitude,
                Latitude = model.Latitude,
                Qr = "1"
            };

            _context.Stores.Add(store);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetStore), new
            {
                id = store.IdStore
            }, store);
        }


        // PUT: api/Stores/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStore(int id, [FromBody] Store model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var store = await _context.Stores.FindAsync(id);
            if (store == null)
            {
                return NotFound();
            }

            store.Name = model.Name;
            store.Address = model.Address;
            store.Longitude = model.Longitude;
            store.Latitude = model.Latitude;
            store.IdClient = model.IdClient;

            _context.Entry(store).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Stores/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStore(int id)
        {
            var store = await _context.Stores.FindAsync(id);
            if (store == null)
            {
                return NotFound();
            }

            _context.Stores.Remove(store);
            await _context.SaveChangesAsync();

            return Ok(store);
        }
    }
}

