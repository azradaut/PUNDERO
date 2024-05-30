using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PUNDERO.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace PUNDERO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvController : ControllerBase
    {
        private readonly PunderoContext _context;
        private readonly ILogger<InvController> _logger;

        public InvController(PunderoContext context, ILogger<InvController> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        private async Task CreateNotification(int accountId, string message, int invoiceId)
        {
            try
            {
                var notification = new Notification
                {
                    IdAccount = accountId,
                    Message = message,
                    Seen = false,
                    CreatedAt = DateTime.Now,
                    IdInvoice = invoiceId
                };

                _context.Notifications.Add(notification);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Notification created successfully for Account ID: {accountId}, Invoice ID: {invoiceId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating notification for Account ID: {accountId}, Invoice ID: {invoiceId}");
                throw;
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetInvoices()
        {
            try
            {
                var invoices = await _context.Invoices
                    .Include(i => i.IdStoreNavigation)
                    .Include(i => i.IdStatusNavigation)
                    .Select(i => new {
                        i.IdInvoice,
                        i.IssueDate,
                        StoreName = i.IdStoreNavigation.Name,
                        StatusName = i.IdStatusNavigation.Name
                    })
                    .ToListAsync();

                return Ok(invoices);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching invoices.");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("store/{storeName}")]
        public async Task<IActionResult> GetInvoicesByStoreName(string storeName)
        {
            try
            {
                var invoices = await _context.Invoices
                    .Include(i => i.IdStoreNavigation)
                    .Include(i => i.IdStatusNavigation)
                    .Where(i => i.IdStoreNavigation.Name == storeName)
                    .Select(i => new {
                        i.IdInvoice,
                        i.IssueDate,
                        StoreName = i.IdStoreNavigation.Name,
                        StatusName = i.IdStatusNavigation.Name
                    })
                    .ToListAsync();

                return Ok(invoices);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching invoices for the store.");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("delivered/{storeName}")]
        public async Task<IActionResult> GetDeliveredInvoicesByStoreName(string storeName)
        {
            try
            {
                var deliveredInvoices = await _context.Invoices
                    .Include(i => i.IdStoreNavigation)
                    .Include(i => i.IdStatusNavigation)
                    .Include(i => i.IdDriverNavigation)
                        .ThenInclude(d => d.IdAccountNavigation)
                    .Include(i => i.InvoiceProducts)
                        .ThenInclude(ip => ip.IdProductNavigation)
                    .Where(i => i.IdStoreNavigation.Name == storeName && i.IdStatus == 5) // Assuming status ID 5 is for delivered
                    .Select(i => new {
                        i.IdInvoice,
                        i.IssueDate,
                        StoreName = i.IdStoreNavigation.Name,
                        StatusName = i.IdStatusNavigation.Name,
                        DriverName = i.IdDriverNavigation != null ? $"{i.IdDriverNavigation.IdAccountNavigation.FirstName} {i.IdDriverNavigation.IdAccountNavigation.LastName}" : null,
                        Products = i.InvoiceProducts.Select(ip => new {
                            ip.IdProductNavigation.NameProduct,
                            ip.OrderQuantity,
                            ip.IdProductNavigation.Price,
                            TotalPrice = ip.OrderQuantity * ip.IdProductNavigation.Price
                        }),
                        TotalAmount = i.InvoiceProducts.Sum(ip => ip.OrderQuantity * ip.IdProductNavigation.Price)
                    })
                    .ToListAsync();

                return Ok(deliveredInvoices);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching delivered invoices for the store.");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetInvoice(int id)
        {
            var invoice = await _context.Invoices
                .Include(i => i.IdStoreNavigation)
                .Include(i => i.IdStatusNavigation)
                .Include(i => i.IdDriverNavigation)
                    .ThenInclude(d => d.IdAccountNavigation)
                .Include(i => i.InvoiceProducts)
                    .ThenInclude(ip => ip.IdProductNavigation)
                .SingleOrDefaultAsync(i => i.IdInvoice == id);

            if (invoice == null)
            {
                return NotFound();
            }

            var invoiceDetails = new
            {
                invoice.IdInvoice,
                invoice.IssueDate,
                StoreName = invoice.IdStoreNavigation?.Name,
                DriverName = invoice.IdDriverNavigation != null ? $"{invoice.IdDriverNavigation.IdAccountNavigation.FirstName} {invoice.IdDriverNavigation.IdAccountNavigation.LastName}" : null,
                StatusName = invoice.IdStatusNavigation?.Name,
                Products = invoice.InvoiceProducts.Select(ip => new
                {
                    ip.IdProductNavigation.NameProduct,
                    ip.OrderQuantity,
                    ip.IdProductNavigation.Price,
                    TotalPrice = ip.OrderQuantity * ip.IdProductNavigation.Price
                }),
                TotalAmount = invoice.InvoiceProducts.Sum(ip => ip.OrderQuantity * ip.IdProductNavigation.Price)
            };

            return Ok(invoiceDetails);
        }

        [HttpGet("deliveredToClient/{storeName}")]
        public async Task<IActionResult> GetDeliveredInvoices(string storeName)
        {
            try
            {
                var invoices = await _context.Invoices
                    .Where(i => i.IdStoreNavigation.Name == storeName && i.IdStatus == 5) // Assuming status ID 5 is for delivered
                    .Include(i => i.IdStoreNavigation)
                    .Include(i => i.IdDriverNavigation)
                        .ThenInclude(d => d.IdAccountNavigation)
                    .Include(i => i.InvoiceProducts)
                        .ThenInclude(ip => ip.IdProductNavigation)
                    .Select(i => new
                    {
                        i.IdInvoice,
                        i.IssueDate,
                        StoreName = i.IdStoreNavigation.Name,
                        DriverName = i.IdDriverNavigation != null ? $"{i.IdDriverNavigation.IdAccountNavigation.FirstName} {i.IdDriverNavigation.IdAccountNavigation.LastName}" : null,
                        Products = i.InvoiceProducts.Select(ip => new
                        {
                            ip.IdProductNavigation.NameProduct,
                            ip.OrderQuantity,
                            ip.IdProductNavigation.Price,
                            TotalPrice = ip.OrderQuantity * ip.IdProductNavigation.Price
                        }),
                        TotalAmount = i.InvoiceProducts.Sum(ip => ip.OrderQuantity * ip.IdProductNavigation.Price),
                        StatusName = i.IdStatusNavigation.Name
                    })
                    .ToListAsync();

                return Ok(invoices);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching delivered invoices.");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}/complete")]
        public async Task<IActionResult> CompleteInvoice(int id)
        {
            var invoice = await _context.Invoices.FindAsync(id);
            if (invoice == null)
            {
                return NotFound();
            }

            invoice.IdStatus = 6; // Completed status ID
            await _context.SaveChangesAsync();

            var client = await _context.Clients
                                       .Include(c => c.IdAccountNavigation)
                                       .FirstOrDefaultAsync(c => c.IdClient == invoice.IdStoreNavigation.IdClient);
            if (client?.IdAccount != null)
            {
                await CreateNotification(client.IdAccount.Value, "Your invoice has been completed.", invoice.IdInvoice);
            }

            return NoContent();
        }

        [HttpPut("{id}/fail")]
        public async Task<IActionResult> FailInvoice(int id)
        {
            var invoice = await _context.Invoices.FindAsync(id);
            if (invoice == null)
            {
                return NotFound();
            }

            invoice.IdStatus = 7; // Failed status ID
            await _context.SaveChangesAsync();

            var client = await _context.Clients
                                       .Include(c => c.IdAccountNavigation)
                                       .FirstOrDefaultAsync(c => c.IdClient == invoice.IdStoreNavigation.IdClient);
            if (client?.IdAccount != null)
            {
                await CreateNotification(client.IdAccount.Value, "Your invoice has been marked as failed.", invoice.IdInvoice);
            }

            return NoContent();
        }


    }

    public class CreateInvoiceRequest
    {
        public int ClientId { get; set; }
        public string StoreName { get; set; }
        public ProductRequest[] Products { get; set; }
    }

    public class ProductRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }

    public class AssignDriverAndWarehouseRequest
    {
        public int WarehouseId { get; set; }
        public int DriverId { get; set; }
    }
}
