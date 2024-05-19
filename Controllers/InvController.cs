using Microsoft.AspNetCore.Mvc;

namespace PUNDERO.Controllers
{
    public class InvController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
