using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PUNDERO.Models;
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

        // GET: api/Client
        [HttpGet]
        public IActionResult GetClients()
        {
            var clients = _context.Clients
                .Include(c => c.IdAccountNavigation)
                .Include(c => c.Stores)
                .Select(c => new
                {
                    c.IdClient,
                    FirstName = c.IdAccountNavigation.FirstName,
                    LastName = c.IdAccountNavigation.LastName,
                    Email = c.IdAccountNavigation.Email,
                    Type = "Client",
                    Store = c.Stores.Select(s => s.Name).FirstOrDefault(),
                    c.IdAccountNavigation.Image
                })
                .ToList();

            return Ok(clients);
        }

        // GET: api/Client/IdAccount
        [HttpGet("{IdAccount}")]
        public IActionResult GetClientByIdAccount(int IdAccount)
        {
            var client = _context.Clients
                .Include(c => c.IdAccountNavigation)
                .Include(c => c.Stores)
                .Where(c => c.IdAccount == IdAccount)
                .Select(c => new
                {
                    c.IdClient,
                    FirstName = c.IdAccountNavigation.FirstName,
                    LastName = c.IdAccountNavigation.LastName,
                    Email = c.IdAccountNavigation.Email,
                    Type = "Client",
                    Store = c.Stores.Select(s => s.Name).FirstOrDefault(),
                    c.IdAccountNavigation.Image
                })
                .FirstOrDefault();

            if (client == null)
            {
                return NotFound();
            }

            return Ok(client);
        }

        // POST: api/Client/AddClient
        // POST: api/Client/AddClient
        [HttpPost]
        public async Task<IActionResult> AddClient([FromBody] ClientViewModel model)
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
                Type = 3, // Client type
                Image = model.Image // Store base64 string directly
            };

            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();

            var client = new Client
            {
                IdAccount = account.IdAccount,
                NameStore = "/" // Default value for NameStore
            };

            _context.Clients.Add(client);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                client.IdClient,
                account.FirstName,
                account.LastName,
                account.Email,
                account.Type,
                Store = client.NameStore,
                account.Image
            });
        }
    
    // PUT: api/Client/UploadImage
    [HttpPost("UploadImage")]
        public async Task<IActionResult> UploadImage()
        {
            var httpRequest = HttpContext.Request;

            if (httpRequest.Form.Files.Count == 0)
            {
                return BadRequest("No file uploaded.");
            }

            var file = httpRequest.Form.Files[0];
            var fileName = httpRequest.Form["fileName"].ToString();

            var path = Path.Combine(Directory.GetCurrentDirectory(), "uploads", fileName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return Ok(new { path });
        }

    }



    public class ClientViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Store { get; set; }
        public string? Image { get; set; }
    }
}
