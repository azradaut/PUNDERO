namespace PUNDERO.Models
{
    public class LoginModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class LoginResponseModel
    {
        public string Token { get; set; }
        public string Role { get; set; }
    }
}
