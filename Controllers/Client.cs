using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PUNDERO.Models;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PUNDERO.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly PunderoContext _context;

        public ClientController(PunderoContext context)
        {
            _context = context;
        }

        // GET: api/Client/GetClients
        [HttpGet]
        public IActionResult GetClients()
        {
            var clients = _context.Clients
                .Include(c => c.IdAccountNavigation)
                .Include(c => c.Stores)
                .Select(c => new ClientViewModel
                {
                    IdClient = c.IdClient,
                    IdAccount = c.IdAccount,
                    FirstName = c.IdAccountNavigation.FirstName,
                    LastName = c.IdAccountNavigation.LastName,
                    Email = c.IdAccountNavigation.Email,
                    Store = c.Stores.Select(s => s.Name).FirstOrDefault(),
                    Image = c.IdAccountNavigation.Image
                })
                .ToList();

            return Ok(clients);
        }

        // GET: api/Client/GetClientByIdAccount/5
        [HttpGet("{id}")]
        public IActionResult GetClientByIdAccount(int id)
        {
            var client = _context.Clients
                .Include(c => c.IdAccountNavigation)
                .Include(c => c.Stores)
                .Where(c => c.IdAccount == id)
                .Select(c => new ClientViewModel
                {
                    IdClient = c.IdClient,
                    IdAccount = c.IdAccount,
                    FirstName = c.IdAccountNavigation.FirstName,
                    LastName = c.IdAccountNavigation.LastName,
                    Email = c.IdAccountNavigation.Email,
                    Store = c.Stores.Select(s => s.Name).FirstOrDefault(),
                    Image = c.IdAccountNavigation.Image
                })
                .FirstOrDefault();

            if (client == null)
            {
                return NotFound();
            }

            return Ok(client);
        }

        // POST: api/Client/AddClient
        [HttpPost]
        public async Task<IActionResult> AddClient([FromForm] ClientViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var account = new Account
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Password = model.Password,
                Type = 3, // Client type
                Image = model.Image
            };

            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();

            var client = new Client
            {
                IdAccount = account.IdAccount,
            };

            _context.Clients.Add(client);
            await _context.SaveChangesAsync();

            return Ok(new ClientViewModel
            {
                IdClient = client.IdClient,
                IdAccount = account.IdAccount,
                FirstName = account.FirstName,
                LastName = account.LastName,
                Email = account.Email,
                Store = model.Store,
                Image = account.Image
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateClient(int id, [FromForm] ClientViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var client = await _context.Clients.FindAsync(id);
            if (client == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts.FindAsync(client.IdAccount);
            if (account == null)
            {
                return NotFound();
            }

            account.FirstName = model.FirstName;
            account.LastName = model.LastName;
            account.Email = model.Email;
            account.Password = model.Password;

            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                var imagePath = Path.Combine("wwwroot", "images", "profile_images", $"{model.FirstName}{model.LastName}.jpg");
                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    await model.ImageFile.CopyToAsync(stream);
                }
                account.Image = $"/images/profile_images/{model.FirstName}{model.LastName}.jpg";
            }

            _context.Entry(account).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }



        // DELETE: api/Client/DeleteClient/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClient(int id)
        {
            var client = await _context.Clients.FindAsync(id);
            if (client == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts.FindAsync(client.IdAccount);
            if (account != null)
            {
                _context.Accounts.Remove(account);
            }

            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/Client/UploadImage
        [HttpPost("UploadImage")]
        public async Task<IActionResult> UploadImage([FromForm] IFormFile file, [FromForm] string fileName)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "profile_images", fileName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return Ok(new { path });
        }
    }
}
