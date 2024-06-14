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
        public async Task<IActionResult> GetStores()
        {
            var stores = await _context.Stores
                .Include(s => s.IdClientNavigation)
                    .ThenInclude(c => c.IdAccountNavigation)
                .Select(s => new StoreViewModel
                {
                    IdStore = s.IdStore,
                    Name = s.Name,
                    Address = s.Address,
                    Longitude = s.Longitude,
                    Latitude = s.Latitude,
                    ClientName = s.IdClientNavigation != null
                        ? s.IdClientNavigation.IdAccountNavigation.FirstName + " " + s.IdClientNavigation.IdAccountNavigation.LastName
                        : "Unassigned Client",
                    Qr = s.Qr
                })
                .ToListAsync();

            return Ok(stores);
        }

        // GET: api/Stores/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetStore(int id)
        {
            var store = await _context.Stores
                .Include(s => s.IdClientNavigation)
                    .ThenInclude(c => c.IdAccountNavigation)
                .Where(s => s.IdStore == id)
                .Select(s => new StoreViewModel
                {
                    IdStore = s.IdStore,
                    Name = s.Name,
                    Address = s.Address,
                    Longitude = s.Longitude,
                    Latitude = s.Latitude,
                    ClientName = s.IdClientNavigation != null
                        ? s.IdClientNavigation.IdAccountNavigation.FirstName + " " + s.IdClientNavigation.IdAccountNavigation.LastName
                        : "Unassigned Client",
                    Qr = s.Qr
                })
                .FirstOrDefaultAsync();

            if (store == null)
            {
                return NotFound();
            }

            return Ok(store);
        }

        // POST: api/Stores
        [HttpPost]
        public async Task<IActionResult> AddStore([FromBody] StoreViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Client? client = null;
            if (!string.IsNullOrEmpty(model.ClientName))
            {
                var clientNameParts = model.ClientName.Split(' ');
                if (clientNameParts.Length >= 2)
                {
                    var firstName = clientNameParts[0];
                    var lastName = clientNameParts[1];
                    client = await _context.Clients
                        .Include(c => c.IdAccountNavigation)
                        .FirstOrDefaultAsync(c => c.IdAccountNavigation.FirstName == firstName
                                                  && c.IdAccountNavigation.LastName == lastName);
                }
            }

            if (client == null && !string.IsNullOrEmpty(model.ClientName))
            {
                return BadRequest("Client not found.");
            }

            var store = new Store
            {
                Name = model.Name,
                Address = model.Address,
                Longitude = model.Longitude,
                Latitude = model.Latitude,
                IdClient = client?.IdClient,
                Qr = "1"
            };

            _context.Stores.Add(store);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetStore), new { id = store.IdStore }, store);
        }

        // PUT: api/Stores/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStore(int id, [FromBody] StoreViewModel model)
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

            Client? client = null;
            if (!string.IsNullOrEmpty(model.ClientName))
            {
                var clientNameParts = model.ClientName.Split(' ');
                if (clientNameParts.Length >= 2)
                {
                    var firstName = clientNameParts[0];
                    var lastName = clientNameParts[1];
                    client = await _context.Clients
                        .Include(c => c.IdAccountNavigation)
                        .FirstOrDefaultAsync(c => c.IdAccountNavigation.FirstName == firstName
                                                  && c.IdAccountNavigation.LastName == lastName);
                }
            }

            if (client == null && !string.IsNullOrEmpty(model.ClientName))
            {
                return BadRequest("Client not found.");
            }

            store.Name = model.Name;
            store.Address = model.Address;
            store.Longitude = model.Longitude;
            store.Latitude = model.Latitude;
            store.IdClient = client?.IdClient;

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
