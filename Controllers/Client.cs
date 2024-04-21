using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PUNDERO.Models;

namespace PUNDERO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly PunderoContext _context;

        public ClientController(PunderoContext context)
        {
            _context = context;
        }
        // GET: api/Client
        [HttpGet]
        public IActionResult GetClients()
        {
            var clients = _context.Clients.ToList();
            return Ok(clients);
        }
        // GET: api/Client/IdAccount
        [HttpGet("{IdAccount}")]
        public IActionResult GetClientByIdAccount(int IdAccount)
        {
            var client = _context.Clients.FirstOrDefault(v => v.IdAccount == IdAccount );

            if (client == null)
            {
                return NotFound();
            }

            return Ok(client);
        }
        // PUT: api/Client/id
        [HttpPut("{id}")]
        public IActionResult PutClient(int id, [FromBody] Client client)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != client.IdClient)
            {
                return BadRequest();
            }

            _context.Entry(client).State = EntityState.Modified;
            _context.SaveChanges();

            return NoContent();
        }
        // POST: api/client
        [HttpPost]
        public IActionResult PostClient([FromBody] Client client)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Clients.Add(client);
            _context.SaveChanges();

            return CreatedAtRoute("GetClient", new { id = client.IdClient }, client);
        }
        // DELETE: api/Client
        [HttpDelete("{id}")]
        public IActionResult DeleteClient(int id)
        {
            var client = _context.Clients.FirstOrDefault(wh => wh.IdClient == id);
            if (client == null)
            {
                return NotFound();
            }

            _context.Clients.Remove(client);
            _context.SaveChanges();

            return Ok(client);
        }

    }
}
