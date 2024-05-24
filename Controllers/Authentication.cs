using Microsoft.AspNetCore.Mvc;
using PUNDERO.Helper;
using PUNDERO.Models;


namespace PUNDERO.Controllers;

[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly PunderoContext _applicationDbContext = new PunderoContext();
    [HttpPost("login")]
    public MyAuthInfo Obradi([FromBody] AuthLoginRequest request)
    {
        //1- provjera logina
        var logiraniKorisnik = _applicationDbContext.Accounts
            .FirstOrDefault(k =>
                k.Email == request.Email && k.Password == request.Password);

        if (logiraniKorisnik == null)
        {
            //pogresan username i password
            return new MyAuthInfo(null);
        }

        //2- generisati random string
        string randomString = TokenGenerator.Generate(10);

        //3- dodati novi zapis u tabelu AutentifikacijaToken za logiraniKorisnikId i randomString
        var noviToken = new AuthenticationToken()
        {
            TokenValue = randomString,
            IdAccountNavigation = logiraniKorisnik,
            SignDate = DateTime.Now,
        };

        _applicationDbContext.Add(noviToken);
        _applicationDbContext.SaveChanges();


        //4- vratiti token string
        return new MyAuthInfo(noviToken);
    }

    public class AuthLoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }


    //Login mobile
    [HttpPost("mobile/login")]
    
    public MyAuthInfoMobile Process([FromBody] AuthLoginRequest request)
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
