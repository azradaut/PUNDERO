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

}