using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PUNDERO.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PUNDERO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvController : ControllerBase
    {
        private readonly PunderoContext _context;

        public InvController(PunderoContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetInvoices()
        {
            var invoices = await _context.Invoices
                .Include(i => i.IdStoreNavigation)
                .Include(i => i.IdWarehouseNavigation)
                .Include(i => i.IdStatusNavigation)
                .Include(i => i.IdDriverNavigation) // Correct the navigation property here
                .Include(i => i.InvoiceProducts)
                    .ThenInclude(ip => ip.IdProductNavigation)
                .ToListAsync();

            return Ok(invoices);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetInvoice(int id)
        {
            var invoice = await _context.Invoices
                .Include(i => i.IdStoreNavigation)
                .Include(i => i.IdWarehouseNavigation)
                .Include(i => i.IdStatusNavigation)
                .Include(i => i.IdDriverNavigation) // Correct the navigation property here
                .Include(i => i.InvoiceProducts)
                    .ThenInclude(ip => ip.IdProductNavigation)
                .SingleOrDefaultAsync(i => i.IdInvoice == id);

            if (invoice == null)
            {
                return NotFound();
            }

            return Ok(invoice);
        }

        [HttpPost]
        public async Task<IActionResult> CreateInvoice(CreateInvoiceRequest request)
        {
            var store = await _context.Stores.SingleOrDefaultAsync(s => s.IdClient == request.ClientId);
            if (store == null)
            {
                return BadRequest("Invalid client ID.");
            }

            var invoice = new Invoice
            {
                IssueDate = DateTime.Now,
                IdStore = store.IdStore,
                IdStatus = 1, // Pending
                InvoiceProducts = request.Products.Select(p => new InvoiceProduct
                {
                    IdProduct = p.ProductId,
                    OrderQuantity = p.Quantity
                }).ToList()
            };

            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();

            return Ok(invoice);
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateInvoiceStatus(int id, UpdateInvoiceStatusRequest request)
        {
            var invoice = await _context.Invoices.FindAsync(id);
            if (invoice == null)
            {
                return NotFound();
            }

            invoice.IdStatus = request.StatusId;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("{id}/assign")]
        public async Task<IActionResult> AssignDriverAndWarehouse(int id, AssignDriverAndWarehouseRequest request)
        {
            var invoice = await _context.Invoices.FindAsync(id);
            if (invoice == null)
            {
                return NotFound();
            }

            invoice.IdWarehouse = request.WarehouseId;
            invoice.IdDriver = request.DriverId;
            invoice.IdStatus = 2; // Approved
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }

    public class CreateInvoiceRequest
    {
        public int ClientId { get; set; }
        public ProductRequest[] Products { get; set; }
    }

    public class ProductRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }

    public class UpdateInvoiceStatusRequest
    {
        public int StatusId { get; set; }
    }

    public class AssignDriverAndWarehouseRequest
    {
        public int WarehouseId { get; set; }
        public int DriverId { get; set; }
    }
}
