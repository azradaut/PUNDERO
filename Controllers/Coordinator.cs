using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PUNDERO.Models;
using System.Linq;
using System.Threading.Tasks;

namespace PUNDERO.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CoordinatorController : ControllerBase
    {
        private readonly PunderoContext _context;

        public CoordinatorController(PunderoContext context)
        {
            _context = context;
        }

        // GET: api/Coordinator/GetCoordinators
        [HttpGet]
        public IActionResult GetCoordinators()
        {
            var coordinators = _context.Coordinators
                .Include(c => c.IdAccountNavigation)
                .Select(c => new
                {
                    c.IdCoordinator,
                    FirstName = c.IdAccountNavigation.FirstName,
                    LastName = c.IdAccountNavigation.LastName,
                    Email = c.IdAccountNavigation.Email,
                    Type = "Coordinator",
                    c.Qualification,
                    c.Description,
                    c.IdAccountNavigation.Image
                })
                .ToList();

            return Ok(coordinators);
        }

        // GET: api/Coordinator/IdAccount
        [HttpGet("{IdAccount}")]
        public IActionResult GetCoordinatorByIdAccount(int IdAccount)
        {
            var coordinator = _context.Coordinators
                .Include(c => c.IdAccountNavigation)
                .Where(c => c.IdAccount == IdAccount)
                .Select(c => new
                {
                    c.IdCoordinator,
                    FirstName = c.IdAccountNavigation.FirstName,
                    LastName = c.IdAccountNavigation.LastName,
                    Email = c.IdAccountNavigation.Email,
                    Type = "Coordinator",
                    c.Qualification,
                    c.Description,
                    c.IdAccountNavigation.Image
                })
                .FirstOrDefault();

            if (coordinator == null)
            {
                return NotFound();
            }

            return Ok(coordinator);
        }

        // POST: api/Coordinator/AddCoordinator
        [HttpPost]
        public async Task<IActionResult> AddCoordinator([FromBody] CoordinatorViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.SelectMany(x => x.Value.Errors.Select(e => e.ErrorMessage)).ToList();
                foreach (var error in errors)
                {
                    Console.WriteLine(error);
                }
                return BadRequest(ModelState);
            }

            var account = new Account
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Password = model.Password,
                Type = 1, // Coordinator type
                Image = model.Image // Store base64 string directly
            };

            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();

            var coordinator = new Coordinator
            {
                Qualification = model.Qualification,
                Description = model.Description,
                IdAccount = account.IdAccount
            };

            _context.Coordinators.Add(coordinator);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                coordinator.IdCoordinator,
                account.FirstName,
                account.LastName,
                account.Email,
                account.Type,
                coordinator.Qualification,
                coordinator.Description,
                account.Image
            });
        }
    }
}