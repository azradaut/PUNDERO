using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PUNDERO.Models;
using System.Linq;
using System.Threading.Tasks;

namespace PUNDERO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientMapController : ControllerBase
    {
        private readonly PunderoContext _context;

        public ClientMapController(PunderoContext context)
        {
            _context = context;
        }

        [HttpGet("store/{storeName}")]
        public async Task<IActionResult> GetStoreByStoreName(string storeName)
        {
            var store = await _context.Stores
                .Where(s => s.Name == storeName)
                .Select(s => new {
                    s.IdStore,
                    s.Name,
                    s.Address,
                    Latitude = s.Latitude,
                    Longitude = s.Longitude
                })
                .FirstOrDefaultAsync();

            if (store == null)
            {
                return NotFound("Store not found.");
            }

            return Ok(store);
        }

        [HttpGet("drivers/{storeName}")]
        public async Task<IActionResult> GetDriversDeliveringToStore(string storeName)
        {
            var drivers = await _context.Invoices
                .Where(i => i.IdStatus == 4 && i.IdStoreNavigation.Name == storeName)
                .Include(i => i.IdDriverNavigation)
                    .ThenInclude(d => d.MobileDrivers)
                    .ThenInclude(md => md.IdMobileNavigation)
                .Select(i => new {
                    DriverId = i.IdDriver,
                    i.IdDriverNavigation.IdAccountNavigation.FirstName,
                    i.IdDriverNavigation.IdAccountNavigation.LastName,
                    MobilePhoneNumber = i.IdDriverNavigation.PrivateMobile,
                    LkLatitude = i.IdDriverNavigation.MobileDrivers.FirstOrDefault().IdMobileNavigation.LkLatitude,
                    LkLongitude = i.IdDriverNavigation.MobileDrivers.FirstOrDefault().IdMobileNavigation.LkLongitude
                })
                .ToListAsync();

            return Ok(drivers);
        }
    }
}
