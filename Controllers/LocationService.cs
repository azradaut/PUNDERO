using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PUNDERO.Helper;
using PUNDERO.Models;
using System.Linq;
using System.Threading.Tasks;

namespace PUNDERO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        private readonly PunderoContext _context;
        private readonly MyAuthService _authService;

        public LocationController(PunderoContext context, MyAuthService authService)
        {
            _context = context;
            _authService = authService;
        }
        public class UpdateLocationDto
        {
            public double Longitude { get; set; }
            public double Latitude { get; set; }
        }


        [HttpPut("update")]
        public async Task<IActionResult> UpdateLocation([FromBody] UpdateLocationDto updateLocationDto)
        {
            if (updateLocationDto == null)
            {
                return BadRequest("Invalid location data.");
            }

            // Retrieve authentication information using your service
            var authInfo = _authService.GetAuthInfo();

            // Check if the user is authenticated
            if (!authInfo.isLogiran)
            {
                return Unauthorized("Unauthorized access.");
            }

            // Find the driver associated with the logged-in account
            var driver = await _context.Drivers
                .Include(d => d.MobileDrivers)
                .ThenInclude(md => md.IdMobileNavigation)
                .FirstOrDefaultAsync(d => d.IdAccount == authInfo.IdAccount);

            if (driver == null)
            {
                return NotFound("Driver not found for the account.");
            }

            // Update the location of the driver's mobile device
            var mobileDriver = driver.MobileDrivers.FirstOrDefault();
            if (mobileDriver == null || mobileDriver.IdMobileNavigation == null)
            {
                return NotFound("No mobile device assigned to the driver.");
            }

            mobileDriver.IdMobileNavigation.LkLongitude = updateLocationDto.Longitude;
            mobileDriver.IdMobileNavigation.LkLatitude = updateLocationDto.Latitude;

            _context.Mobiles.Update(mobileDriver.IdMobileNavigation);
            await _context.SaveChangesAsync();

            return Ok("Location updated successfully.");
        }
    }
}
