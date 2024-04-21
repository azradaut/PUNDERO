using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PUNDERO.Models;

namespace PUNDERO.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MobileController : ControllerBase
    {
        private readonly PunderoContext _context;

        public MobileController(PunderoContext context)
        {
            _context = context;
        }

        // GET: api/Mobile
        [HttpGet]
        public IActionResult GetMobiles()
        {
            var mobiles = _context.Mobiles.OrderByDescending(m => m.PhoneNumber).ToList();
            return Ok(mobiles);
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

        // POST: api/Mobile
        [HttpPost]
        public IActionResult PostMobile([FromBody] Mobile mobile)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Mobiles.Add(mobile);
            _context.SaveChanges();

            return CreatedAtRoute("GetMobile", new { phoneNumber = mobile.PhoneNumber }, mobile);
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

        // DELETE: api/Mobile/1234567890 (Assuming phone number is an int)
        [HttpDelete("{phoneNumber}")]
        public IActionResult DeleteMobile(int phoneNumber) // Adjusted the parameter type to int
        {
            var mobile = _context.Mobiles.FirstOrDefault(m => m.PhoneNumber == phoneNumber);
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
