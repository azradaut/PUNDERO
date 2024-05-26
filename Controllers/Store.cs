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
    [Route("api/[controller]/[action]")]
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

        // GET: api/Stores
        [HttpGet]
        public IActionResult GetStores()
        {
            var stores = _context.Stores.OrderByDescending(s => s.IdStore).ToList();
            return Ok(stores);
        }

        // GET: api/Stores/5
        [HttpGet("{id}")]
        public IActionResult GetStores(int id)
        {
            var store = _context.Stores.FirstOrDefault(s => s.IdStore == id);

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

        // POST: api/Store
        [HttpPost]
        public IActionResult PostStore([FromBody] Store store)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Stores.Add(store);
            _context.SaveChanges();

            return CreatedAtRoute("GetStore", new { id = store.IdStore }, store);
        }

        // PUT: api/Store/5
        [HttpPut("{id}")]
        public IActionResult PutStore(int id, [FromBody] Store store)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != store.IdStore)
            {
                return BadRequest();
            }

            _context.Entry(store).State = EntityState.Modified;
            _context.SaveChanges();

            return NoContent();
        }

        // DELETE: api/Store/5
        [HttpDelete("{id}")]
        public IActionResult DeleteStore(int id)
        {
            var store = _context.Stores.FirstOrDefault(s => s.IdStore == id);
            if (store == null)
            {
                return NotFound();
            }

            _context.Stores.Remove(store);
            _context.SaveChanges();

            return Ok(store);
        }
    }
}
