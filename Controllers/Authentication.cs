using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using PUNDERO.Models;
using System.Linq;
using PUNDERO.Helper;

namespace PUNDERO.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly PunderoContext _applicationDbContext = new PunderoContext();
        private readonly PunderoContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(PunderoContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel login)
        {
            var user = _context.Accounts.SingleOrDefault(u => u.Email == login.Email && u.Password == login.Password);

            if (user == null)
            {
                return Unauthorized("Invalid credentials");
            }

            var role = GetRoleFromType(user.Type);

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.IdAccount.ToString()),
                    new Claim(ClaimTypes.Role, role)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new LoginResponseModel
            {
                Token = tokenString,
                Role = role
            });
        }

        private string GetRoleFromType(int type)
        {
            return type switch
            {
                1 => "Coordinator",
                3 => "Client",
                _ => "Unknown"
            };
        }

        //Login mobile
        [HttpPost("mobile/login")]

        public MyAuthInfoMobile Process([FromBody] AuthLoginRequestMobile request)
        {
            var logiraniKorisnik = _applicationDbContext.Accounts
                .FirstOrDefault(k => k.Email == request.Email && k.Password == request.Password);

            if (logiraniKorisnik == null)
            {
                return null;
            }

            string randomString = TokenGenerator.Generate(10);

            var noviToken = new AuthenticationToken()
            {
                TokenValue = randomString,
                IdAccountNavigation = logiraniKorisnik,
                SignDate = DateTime.Now,
            };

            _applicationDbContext.Add(noviToken);
            _applicationDbContext.SaveChanges();

            int? driverId = _applicationDbContext.Drivers
                .Where(d => d.IdAccount == logiraniKorisnik.IdAccount)
                .Select(d => (int?)d.IdDriver)
                .FirstOrDefault();

            return new MyAuthInfoMobile(noviToken, driverId);
        }

        public class AuthLoginRequestMobile
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }
    }

    public class MyAuthInfoMobile
    {
        public MyAuthInfoMobile(AuthenticationToken? autentifikacijaToken, int? driverId)
        {
            this.IdAccount = autentifikacijaToken?.IdAccountNavigation?.IdAccount;
            this.Email = autentifikacijaToken?.IdAccountNavigation?.Email;
            this.Type = autentifikacijaToken?.IdAccountNavigation?.Type;
            this.TokenValue = autentifikacijaToken?.TokenValue;
            this.DriverId = driverId;
        }

        public int? IdAccount { get; set; }
        public string? Email { get; set; }
        public int? Type { get; set; }
        public string? TokenValue { get; set; }
        public int? DriverId { get; set; }
        public bool isLogiran => IdAccount != null;
    }
}

