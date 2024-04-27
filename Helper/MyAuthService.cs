using Microsoft.EntityFrameworkCore;
using PUNDERO.Models;

namespace PUNDERO.Helper
{
    public class MyAuthService
    {
        private readonly PunderoContext _applicationDbContext = new PunderoContext();
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MyAuthService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public bool IsLogiran => GetAuthInfo().isLogiran;
        public int? UserType => GetAuthInfo().Type;

        public MyAuthInfo GetAuthInfo()
        {
            string? authToken = _httpContextAccessor.HttpContext!.Request.Headers["my-auth-token"];

            AuthenticationToken? autentifikacijaToken = _applicationDbContext.AuthenticationTokens
                .Include(x => x.IdAccountNavigation)
                .SingleOrDefault(x => x.TokenValue == authToken);

            return new MyAuthInfo(autentifikacijaToken);
        }
    }



        public class MyAuthInfo
        {
            public MyAuthInfo(AuthenticationToken? autentifikacijaToken)
            {
                this.IdAccount = autentifikacijaToken?.IdAccountNavigation?.IdAccount;
                this.Email = autentifikacijaToken?.IdAccountNavigation?.Email;
                this.Type = autentifikacijaToken?.IdAccountNavigation?.Type;
                this.TokenValue = autentifikacijaToken?.TokenValue;
            }


            public int? IdAccount { get; set; }

            public string? Email { get; set; }

            public int? Type { get; set; }

            public string? TokenValue { get; set; }

            public bool isLogiran => IdAccount != null;
        }
    }
