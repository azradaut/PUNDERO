using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PUNDERO.Models;
using PUNDERO.Helper;

namespace PUNDERO.Controllers
{

    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        PunderoContext db = new PunderoContext();
        private readonly MyAuthService _authService;


        public AccountController(MyAuthService authService)
        {
            _authService = authService;
        }

        // GET: api/Accounts
        [HttpGet]
        public IActionResult GetAccounts()
        {
            
            var accounts = db.Accounts.ToList();
            return Ok(accounts);
        }
        // GET: api/Accounts/email
        [HttpGet("{email}")]
        public IActionResult GetAccountByEmail(string email)
        {
            var account = db.Accounts.FirstOrDefault(v => v.Email == email);

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

            db.Accounts.Add(account);
            db.SaveChanges();

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

            db.Entry(account).State = EntityState.Modified;
            db.SaveChanges();

            return NoContent();
        }
        // DELETE: api/Account
        [HttpDelete("{id}")]
        public IActionResult DeleteAccount(int id)
        {
            var account = db.Accounts.FirstOrDefault(wh => wh.IdAccount == id);
            if (account == null)
            {
                return NotFound();
            }

            db.Accounts.Remove(account);
            db.SaveChanges();

            return Ok(account);
        }

    } 
}
