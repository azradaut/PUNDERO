using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PUNDERO.Models;

namespace PUNDERO.Controllers
{
  
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly PunderoContext _context;

        public AccountController(PunderoContext context)
        {
            _context = context;
        }
        // GET: api/Accounts
        [HttpGet]
        public IActionResult GetAccounts()
        {
            var accounts = _context.Accounts.ToList();
            return Ok(accounts);
        }
        // GET: api/Accounts/email
        [HttpGet("{email}")]
        public IActionResult GetAccountByEmail(string email)
        {
            var account = _context.Accounts.FirstOrDefault(v => v.Email == email);

            if (account == null)
            {
                return NotFound();
            }

            return Ok(account);
        }
        // POST: api/Accounts
        [HttpPost]
        public IActionResult PostAccount([FromBody] Account account)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Accounts.Add(account);
            _context.SaveChanges();

            return CreatedAtRoute("GetAccountsByEmail", new { email = account.Email }, account);
        }
        // PUT: api/Account/id
        [HttpPut("{id}")]
        public IActionResult PutAccount(int id, [FromBody] Account account)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != account.IdAccount)
            {
                return BadRequest();
            }

            _context.Entry(account).State = EntityState.Modified;
            _context.SaveChanges();

            return NoContent();
        }
        // DELETE: api/Account
        [HttpDelete("{id}")]
        public IActionResult DeleteAccount(int id)
        {
            var account = _context.Accounts.FirstOrDefault(wh => wh.IdAccount == id);
            if (account == null)
            {
                return NotFound();
            }

            _context.Accounts.Remove(account);
            _context.SaveChanges();

            return Ok(account);
        }

    } 
}
