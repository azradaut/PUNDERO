using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PUNDERO.Models;
using System.Linq;
using System.Threading.Tasks;

namespace PUNDERO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MobileController : ControllerBase
    {
        private readonly PunderoContext _context;

        public MobileController(PunderoContext context)
        {
            _context = context;
        }

       



        // GET: api/Mobiles
        [HttpGet]
        public IActionResult GetMobilesCoordinator()
        {
            var mobiles = _context.Mobiles
                .Include(m => m.MobileDrivers)
                    .ThenInclude(md => md.IdDriverNavigation)
                        .ThenInclude(d => d.IdAccountNavigation)
                .Include(m => m.MobileDrivers)
                    .ThenInclude(md => md.IdAssignmentTypeNavigation)
                .Select(m => new
                {
                    m.IdMobile,
                    m.PhoneNumber,
                    m.Brand,
                    m.Model,
                    m.Imei,
                    AssignedDriver = m.MobileDrivers.Select(md => new
                    {
                        IdDriver = md.IdDriver ?? 0,
                        DriverName = md.IdDriverNavigation != null
                            ? md.IdDriverNavigation.IdAccountNavigation.FirstName + " " + md.IdDriverNavigation.IdAccountNavigation.LastName
                            : "Unassigned"
                    }).FirstOrDefault() ?? new { IdDriver = 0, DriverName = "Unassigned" },
                    AssignmentType = m.MobileDrivers.Select(md => md.IdAssignmentTypeNavigation != null
                        ? md.IdAssignmentTypeNavigation.Description
                        : "Unassigned").FirstOrDefault() ?? "Unassigned"
                })
                .ToList();

            return Ok(mobiles);
        }

        // GET: api/Mobiles/{id}
        [HttpGet("{id}")]
        public IActionResult GetMobileCoordinator(int id)
        {
            var mobile = _context.Mobiles
                .Include(m => m.MobileDrivers)
                    .ThenInclude(md => md.IdDriverNavigation)
                        .ThenInclude(d => d.IdAccountNavigation)
                .Include(m => m.MobileDrivers)
                    .ThenInclude(md => md.IdAssignmentTypeNavigation)
                .Where(m => m.IdMobile == id)
                .Select(m => new
                {
                    m.IdMobile,
                    m.PhoneNumber,
                    m.Brand,
                    m.Model,
                    m.Imei,
                    AssignedDriver = m.MobileDrivers.Select(md => new
                    {
                        IdDriver = md.IdDriver ?? 0,
                        DriverName = md.IdDriverNavigation != null
                            ? md.IdDriverNavigation.IdAccountNavigation.FirstName + " " + md.IdDriverNavigation.IdAccountNavigation.LastName
                            : "Unassigned"
                    }).FirstOrDefault() ?? new { IdDriver = 0, DriverName = "Unassigned" },
                    AssignmentType = m.MobileDrivers.Select(md => md.IdAssignmentTypeNavigation != null
                        ? md.IdAssignmentTypeNavigation.Description
                        : "Unassigned").FirstOrDefault() ?? "Unassigned"
                })
                .FirstOrDefault();

            if (mobile == null)
            {
                return NotFound();
            }

            return Ok(mobile);
        }

        // GET: api/Mobile/1234567890 (Assuming phone number is an int)
        [HttpGet("{phoneNumber}")]
        public IActionResult GetMobile(int phoneNumber) // Adjusted the parameter type to int
        {
            var mobile = _context.Mobiles.FirstOrDefault(m => m.PhoneNumber == phoneNumber);

            if (mobile == null)
            {
                return NotFound();
            }

            return Ok(mobile);
        }

        // POST: api/Mobiles
        [HttpPost]
        public IActionResult PostMobile([FromBody] Mobile mobile)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Mobiles.Add(mobile);
            _context.SaveChanges();

            return CreatedAtAction("GetMobile", new { id = mobile.IdMobile }, mobile);
        }

        // PUT: api/Mobiles/{id}
        [HttpPut("{id}")]
        public IActionResult PutMobileCoordinator(int id, [FromBody] Mobile mobile)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != mobile.IdMobile)
            {
                return BadRequest();
            }

            _context.Entry(mobile).State = EntityState.Modified;
            _context.SaveChanges();

            return NoContent();
        }

        // PUT: api/Mobile/1234567890 (Assuming phone number is an int)
        [HttpPut("{phoneNumber}")]
        public IActionResult PutMobile(int phoneNumber, [FromBody] Mobile mobile) // Adjusted the parameter type to int
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (phoneNumber != mobile.PhoneNumber)
            {
                return BadRequest();
            }

            _context.Entry(mobile).State = EntityState.Modified;
            _context.SaveChanges();

            return NoContent();
        }

        // DELETE: api/Mobiles/{id}
        [HttpDelete("{id}")]
        public IActionResult DeleteMobile(int id)
        {
            var mobile = _context.Mobiles.FirstOrDefault(i => i.IdMobile == id);
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
