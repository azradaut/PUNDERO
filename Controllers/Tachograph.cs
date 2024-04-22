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
    public class TachographController : ControllerBase
    {
        private readonly PunderoContext _context;

        public TachographController(PunderoContext context)
        {
            _context = context;
        }

        // GET: api/Tachograph
        [HttpGet]
        public IActionResult GetTachographs()
        {
            var tachographs = _context.Tachographs.OrderByDescending(t => t.Label).ToList();
            return Ok(tachographs);
        }

        // GET: api/Tachograph/my-label
        [HttpGet("{label}")]
        public IActionResult GetTachograph(string label)
        {
            var tachograph = _context.Tachographs.FirstOrDefault(t => t.Label == label);

            if (tachograph == null)
            {
                return NotFound();
            }

            return Ok(tachograph);
        }

        // POST: api/Tachograph
        [HttpPost]
        public IActionResult PostTachograph([FromBody] Tachograph tachograph)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Tachographs.Add(tachograph);
            _context.SaveChanges();

            return CreatedAtRoute("GetTachograph", new { label = tachograph.Label }, tachograph);
        }

        // PUT: api/Tachograph/my-label
        [HttpPut("{label}")]
        public IActionResult PutTachograph(string label, [FromBody] Tachograph tachograph)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (label != tachograph.Label)
            {
                return BadRequest();
            }

            _context.Entry(tachograph).State = EntityState.Modified;
            _context.SaveChanges();

            return NoContent();
        }

        // DELETE: api/Tachograph/my-label
        [HttpDelete("{label}")]
        public IActionResult DeleteTachograph(string label)
        {
            var tachograph = _context.Tachographs.FirstOrDefault(t => t.Label == label);
            if (tachograph == null)
            {
                return NotFound();
            }

            _context.Tachographs.Remove(tachograph);
            _context.SaveChanges();

            return Ok(tachograph);
        }
    }
}
