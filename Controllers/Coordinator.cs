using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PUNDERO.Models;
using System.IO;

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
                .Select(c => new CoordinatorViewModel
                {
                    IdCoordinator = c.IdCoordinator,
                    IdAccount = c.IdAccount ?? 0,
                    FirstName = c.IdAccountNavigation.FirstName,
                    LastName = c.IdAccountNavigation.LastName,
                    Email = c.IdAccountNavigation.Email,
                    Qualification = c.Qualification,
                    Description = c.Description,
                    Image = c.IdAccountNavigation.Image
                })
                .ToList();

            return Ok(coordinators);
        }

        // GET: api/Coordinator/GetCoordinatorByIdAccount/5
        [HttpGet("{id}")]
        public IActionResult GetCoordinatorByIdAccount(int id)
        {
            var coordinator = _context.Coordinators
                .Include(c => c.IdAccountNavigation)
                .Where(c => c.IdAccount == id)
                .Select(c => new CoordinatorViewModel
                {
                    IdCoordinator = c.IdCoordinator,
                    IdAccount = c.IdAccount ?? 0,
                    FirstName = c.IdAccountNavigation.FirstName,
                    LastName = c.IdAccountNavigation.LastName,
                    Email = c.IdAccountNavigation.Email,
                    Password = c.IdAccountNavigation.Password,  // Include password
                    Qualification = c.Qualification,
                    Description = c.Description,
                    Image = c.IdAccountNavigation.Image
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
        public async Task<IActionResult> AddCoordinator([FromForm] CoordinatorViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string imagePath = null;

            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                imagePath = Path.Combine("wwwroot", "images", "profile_images", $"{model.FirstName}{model.LastName}.jpg");
                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    await model.ImageFile.CopyToAsync(stream);
                }
            }

            var account = new Account
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Password = model.Password,
                Type = 1, // Coordinator type
                Image = imagePath != null ? $"/images/profile_images/{model.FirstName}{model.LastName}.jpg" : null
            };

            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();

            var coordinator = new Coordinator
            {
                IdAccount = account.IdAccount,
                Qualification = model.Qualification,
                Description = model.Description
            };

            _context.Coordinators.Add(coordinator);
            await _context.SaveChangesAsync();

            return Ok(new CoordinatorViewModel
            {
                IdCoordinator = coordinator.IdCoordinator,
                IdAccount = account.IdAccount,
                FirstName = account.FirstName,
                LastName = account.LastName,
                Email = account.Email,
                Qualification = coordinator.Qualification,
                Description = coordinator.Description,
                Image = account.Image
            });
        }

        // PUT: api/Coordinator/UpdateCoordinator/5
        [HttpPut("{accountId}")]
        public async Task<IActionResult> UpdateCoordinator(int accountId, [FromForm] CoordinatorViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var coordinator = await _context.Coordinators.FirstOrDefaultAsync(c => c.IdAccount == accountId);
            if (coordinator == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts.FindAsync(accountId);
            if (account == null)
            {
                return NotFound();
            }

            account.FirstName = model.FirstName;
            account.LastName = model.LastName;
            account.Email = model.Email;

            // Retain existing password if not provided
            if (!string.IsNullOrEmpty(model.Password))
            {
                account.Password = model.Password;
            }

            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                var imagePath = Path.Combine("wwwroot", "images", "profile_images", $"{model.FirstName}{model.LastName}.jpg");
                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    await model.ImageFile.CopyToAsync(stream);
                }
                account.Image = $"/images/profile_images/{model.FirstName}{model.LastName}.jpg";
            }

            coordinator.Qualification = model.Qualification;
            coordinator.Description = model.Description;

            _context.Entry(account).State = EntityState.Modified;
            _context.Entry(coordinator).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Coordinator/DeleteCoordinator/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCoordinator(int id)
        {
            var coordinator = await _context.Coordinators.FindAsync(id);
            if (coordinator == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts.FindAsync(coordinator.IdAccount);
            if (account != null)
            {
                _context.Accounts.Remove(account);
            }

            _context.Coordinators.Remove(coordinator);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
