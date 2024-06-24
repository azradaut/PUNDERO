using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PUNDERO.Helper;
using PUNDERO.Models;

namespace PUNDERO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MobileController : ControllerBase
    {
        PunderoContext _context = new PunderoContext();
        private readonly MyAuthService _authService;
        public MobileController(MyAuthService authService)
        {
            _authService = authService;
        }

        // GET: api/Mobile
        [HttpGet]
        public async Task<IActionResult> GetMobiles()
        {
            if (_authService.UserType != 1)
            {
                return Unauthorized("You are not authorized to access this resource.");
            }
            var mobiles = await _context.Mobiles.ToListAsync();
            return Ok(mobiles);
        }

        // GET: api/Mobile/{id}
        [HttpGet("{id}")]
        public IActionResult GetMobile(int id)
        {
            var mobile = _context.Mobiles.Find(id);

            if (mobile == null)
            {
                return NotFound();
            }

            return Ok(mobile);
        }

        // POST: api/Mobile
        [HttpPost]
        public IActionResult PostMobile([FromBody] Mobile mobile)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (_authService.UserType != 1)
            {
                return Unauthorized("You are not authorized to access this resource.");
            }

            _context.Mobiles.Add(mobile);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetMobile), new { id = mobile.IdMobile }, mobile);
        }

        // PUT: api/Mobile/{id}
        [HttpPut("Update/{id}")]
        public IActionResult PutMobile(int id, [FromBody] Mobile mobile)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != mobile.IdMobile)
            {
                return BadRequest();
            }

            if (_authService.UserType != 1)
            {
                return Unauthorized("You are not authorized to access this resource.");
            }

            _context.Entry(mobile).State = EntityState.Modified;
            _context.SaveChanges();

            return NoContent();
        }

        // DELETE: api/Mobile/{id}
        [HttpDelete("{id}")]
        public IActionResult DeleteMobile(int id)
        {
            if (_authService.UserType != 1)
            {
                return Unauthorized("You are not authorized to access this resource.");
            }
            var mobile = _context.Mobiles.Find(id);
            if (mobile == null)
            {
                return NotFound();
            }


            _context.Mobiles.Remove(mobile);
            _context.SaveChanges();

            return Ok(mobile);
        }
    }
}
