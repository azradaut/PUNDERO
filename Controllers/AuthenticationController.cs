using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PUNDERO.Helper;
using PUNDERO.Models;

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
            var account = await _context.Accounts.SingleOrDefaultAsync(a => a.Email == request.Email && a.Password == request.Password);

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

            return Ok(new { Token = token, Type = account.Type });
        }

    }

    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
