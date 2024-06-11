using System;
using Microsoft.AspNetCore.Http;

namespace PUNDERO.Models
{
    public class CoordinatorViewModel
    {
        public int IdCoordinator { get; set; }
        public int IdAccount { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Qualification { get; set; }
        public string Description { get; set; }
        public string? Image { get; set; }
        public IFormFile? ImageFile { get; set; }
    }
}
