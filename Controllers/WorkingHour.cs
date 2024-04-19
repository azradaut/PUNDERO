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
    public class WorkingHoursController : ControllerBase
    {
        private readonly PunderoContext _context;

        public WorkingHoursController(PunderoContext context)
        {
            _context = context;
        }

        // GET: api/WorkingHours
        [HttpGet]
        public IActionResult GetWorkingHours()
        {
            var workingHours = _context.WorkingHours.OrderByDescending(wh => wh.IdWorkingHours).ToList();
            return Ok(workingHours);
        }

        // GET: api/WorkingHours/5
        [HttpGet("{id}")]
        public IActionResult GetWorkingHour(int id)
        {
            var workingHour = _context.WorkingHours.FirstOrDefault(wh => wh.IdWorkingHours == id);

            if (workingHour == null)
            {
                return NotFound();
            }

            return Ok(workingHour);
        }

        // POST: api/WorkingHours
        [HttpPost]
        public IActionResult PostWorkingHour([FromBody] WorkingHour workingHour)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.WorkingHours.Add(workingHour);
            _context.SaveChanges();

            return CreatedAtRoute("GetWorkingHour", new { id = workingHour.IdWorkingHours }, workingHour);
        }

        // PUT: api/WorkingHours/5
        [HttpPut("{id}")]
        public IActionResult PutWorkingHour(int id, [FromBody] WorkingHour workingHour)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != workingHour.IdWorkingHours)
            {
                return BadRequest();
            }

            _context.Entry(workingHour).State = EntityState.Modified;
            _context.SaveChanges();

            return NoContent();
        }

        // DELETE: api/WorkingHours/5
        [HttpDelete("{id}")]
        public IActionResult DeleteWorkingHour(int id)
        {
            var workingHour = _context.WorkingHours.FirstOrDefault(wh => wh.IdWorkingHours == id);
            if (workingHour == null)
            {
                return NotFound();
            }

            _context.WorkingHours.Remove(workingHour);
            _context.SaveChanges();

            return Ok(workingHour);
        }
    }
}
