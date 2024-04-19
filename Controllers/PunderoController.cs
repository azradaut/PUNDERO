using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using PUNDERO.Models;

namespace PUNDERO.Controllers
{
    public class PunderoController : Controller
    {
        PunderoContext db = new PunderoContext();
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }
        //Pojavi se samo ovaj u swaggeru, bez njega ga nema nikako nase baze



    }
}