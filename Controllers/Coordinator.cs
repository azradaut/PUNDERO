using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PUNDERO.Models;

namespace PUNDERO.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CoordinatorController : ControllerBase
    {
        private readonly PunderoContext _context;
        private readonly ILogger<CoordinatorController> _logger;

        public CoordinatorController(PunderoContext context, ILogger<CoordinatorController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Other methods...

        // DELETE: api/Coordinator/DeleteCoordinator/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCoordinator(int id)
        {
            var coordinator = await _context.Coordinators
                .Include(c => c.IdAccountNavigation)
                .FirstOrDefaultAsync(c => c.IdCoordinator == id);

            if (coordinator == null)
            {
                return NotFound();
            }

            try
            {
                var account = coordinator.IdAccountNavigation;

                _context.Coordinators.Remove(coordinator);
                _context.Accounts.Remove(account);

                await _context.SaveChangesAsync();

                return Ok(new { message = "Coordinator deleted successfully." });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error deleting coordinator: " + ex.Message);
                Console.WriteLine(ex.StackTrace);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
