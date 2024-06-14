using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PUNDERO.Helper;
using PUNDERO.Models;
using System.Threading.Tasks;

namespace PUNDERO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly PunderoContext _context;

        public AuthenticationController(PunderoContext context)
        {
            _context = context;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var account = await _context.Accounts
                .SingleOrDefaultAsync(a => a.Email == request.Email && a.Password == request.Password);

            if (account == null)
            {
                return Unauthorized("Invalid email or password.");
            }

            var token = TokenGenerator.Generate(32);
            var authToken = new AuthenticationToken
            {
                TokenValue = token,
                IdAccount = account.IdAccount,
                Email = account.Email,
                SignDate = DateTime.Now
            };

            _context.AuthenticationTokens.Add(authToken);
            await _context.SaveChangesAsync();

            var storeName = string.Empty;
            var clientId = 0;  // Initialize clientId

            if (account.Type == 3)
            {
                var client = await _context.Clients
                    .Include(c => c.Stores)
                    .SingleOrDefaultAsync(c => c.IdAccount == account.IdAccount);

                if (client != null)
                {
                    clientId = client.IdClient;  // Set the clientId
                    if (client.Stores.Any())
                    {
                        storeName = client.Stores.First().Name;
                    }
                }
            }

            return Ok(new
            {
                Token = token,
                Type = account.Type,
                FirstName = account.FirstName,
                LastName = account.LastName,
                StoreName = storeName,
                ClientId = clientId,
                IdAccount = account.IdAccount // Add IdAccount to the response
            });
        }

        public class LoginRequest
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }
    }
}
