using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace PUNDERO.Models
{
    public class ClientViewModel
    {
        public int? IdClient { get; set; }
        public int? IdAccount { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        public string Store { get; set; }
        public string? Image { get; set; }
        public IFormFile? ImageFile { get; set; }
    }
}
