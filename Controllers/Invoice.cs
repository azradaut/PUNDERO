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
    public class InvoiceController : ControllerBase
    {
        private readonly PunderoContext _context;

        public InvoiceController(PunderoContext context)
        {
            _context = context;
        }

        // GET: api/Invoice
        [HttpGet]
        public IActionResult GetInvoices()
        {
            var invoices = _context.Invoices.ToList();
            return Ok(invoices);
        }

        // GET: api/Invoice/1
        [HttpGet("{id}")]
        public IActionResult GetInvoice(int id)
        {
            var invoice = _context.Invoices.FirstOrDefault(v => v.IdInvoice == id);

            if (invoice == null)
            {
                return NotFound();
            }

            return Ok(invoice);
        }

        // POST: api/Invoice
        [HttpPost]
        public IActionResult PostInvoice([FromBody] Invoice invoice)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Invoices.Add(invoice);
            _context.SaveChanges();

            return CreatedAtRoute("GetInvoice", new { id = invoice.IdInvoice }, invoice);
        }

        // PUT: api/Invoice/1
        [HttpPut("{id}")]
        public IActionResult PutInvoice(int id, [FromBody] Invoice invoice)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != invoice.IdInvoice)
            {
                return BadRequest();
            }

            _context.Entry(invoice).State = EntityState.Modified;
            _context.SaveChanges();

            return NoContent();
        }

        // DELETE: api/Invoice/1
        [HttpDelete("{id}")]
        public IActionResult DeleteInvoice(int id)
        {
            var invoice = _context.Invoices.FirstOrDefault(i => i.IdInvoice == id);
            if (invoice == null)
            {
                return NotFound();
            }

            _context.Invoices.Remove(invoice);
            _context.SaveChanges();

            return Ok(invoice);
        }
    }
}
