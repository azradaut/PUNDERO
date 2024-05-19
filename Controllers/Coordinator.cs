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
    public class CoordinatorController : ControllerBase
    {
        private readonly PunderoContext _context;

        public CoordinatorController(PunderoContext context)
        {
            _context = context;
        }

        // GET: api/Coordinator
        [HttpGet]
        public IActionResult GetCoordinators()
        {
            var coordinators = _context.Coordinators.OrderByDescending(c => c.IdCoordinator)
                                                  .Select(c => new // Create an anonymous object to exclude IdAccountNavigation
                                                  {
                                                      IdCoordinator = c.IdCoordinator,
                                                      Qualification = c.Qualification,
                                                      Description = c.Description,
                                                      IdAccount = c.IdAccount
                                                  })
                                                  .ToList();
            return Ok(coordinators);
        }




        // GET: api/Coordinator/IdAccount
        [HttpGet("{IdAccount}")]
        public IActionResult GetCoordinatorByIdAccount(int IdAccount)
        {
            var coordinator = _context.Coordinators.FirstOrDefault(v => v.IdAccount == IdAccount);

            if (coordinator == null)
            {
                return NotFound();
            }

            return Ok(coordinator);
        }
        // POST: api/Coordinator
        [HttpPost]
        public IActionResult PostCoordinator([FromBody] Coordinator coordinator)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Coordinators.Add(coordinator);
            _context.SaveChanges();

            return CreatedAtRoute("GetCoordinator", new { id = coordinator.IdCoordinator }, coordinator);
        }

        // PUT: api/Coordinator/1
        [HttpPut("{id}")]
        public IActionResult PutCoordinator(int id, [FromBody] Coordinator coordinator)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != coordinator.IdCoordinator)
            {
                return BadRequest();
            }

            _context.Entry(coordinator).State = EntityState.Modified;
            _context.SaveChanges();

            return NoContent();
        }

        // DELETE: api/Coordinator/1
        [HttpDelete("{id}")]
        public IActionResult DeleteCoordinator(int id)
        {
            var coordinator = _context.Coordinators.FirstOrDefault(c => c.IdCoordinator == id);
            if (coordinator == null)
            {
                return NotFound();
            }

            _context.Coordinators.Remove(coordinator);
            _context.SaveChanges();

            return Ok(coordinator);
        }
    }
}
