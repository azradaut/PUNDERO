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
    public class InvoiceStatusController : ControllerBase
    {
        private readonly PunderoContext _context;

        public InvoiceStatusController(PunderoContext context)
        {
            _context = context;
        }

        // GET: api/InvoiceStatus
        [HttpGet]
        public IActionResult GetInvoiceStatuses()
        {
            var invoiceStatuses = _context.InvoiceStatuses.OrderByDescending(s => s.IdStatus).ToList();
            return Ok(invoiceStatuses);
        }

        // GET: api/InvoiceStatus/1
        [HttpGet("{id}")]
        public IActionResult GetInvoiceStatus(int id)
        {
            var invoiceStatus = _context.InvoiceStatuses.FirstOrDefault(s => s.IdStatus == id);

            if (invoiceStatus == null)
            {
                return NotFound();
            }

            return Ok(invoiceStatus);
        }

        // POST: api/InvoiceStatus
        [HttpPost]
        public IActionResult PostInvoiceStatus([FromBody] InvoiceStatus invoiceStatus)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.InvoiceStatuses.Add(invoiceStatus);
            _context.SaveChanges();

            return CreatedAtRoute("GetInvoiceStatus", new { id = invoiceStatus.IdStatus }, invoiceStatus);
        }

        // PUT: api/InvoiceStatus/1
        [HttpPut("{id}")]
        public IActionResult PutInvoiceStatus(int id, [FromBody] InvoiceStatus invoiceStatus)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != invoiceStatus.IdStatus)
            {
                return BadRequest();
            }

            _context.Entry(invoiceStatus).State = EntityState.Modified;
            _context.SaveChanges();

            return NoContent();
        }

        // DELETE: api/InvoiceStatus/1
        [HttpDelete("{id}")]
        public IActionResult DeleteInvoiceStatus(int id)
        {
            var invoiceStatus = _context.InvoiceStatuses.FirstOrDefault(s => s.IdStatus == id);
            if (invoiceStatus == null)
            {
                return NotFound();
            }

            _context.InvoiceStatuses.Remove(invoiceStatus);
            _context.SaveChanges();

            return Ok(invoiceStatus);
        }
    }
}
