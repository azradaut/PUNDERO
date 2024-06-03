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
            _context = context;
            _logger = logger;
        }

        private async Task CreateNotification(int accountId, string message)
        {
            var notification = new Notification
            {
                IdAccount = accountId,
                Message = message,
                Seen = false,
                CreatedAt = DateTime.Now
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
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

        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingInvoices()
        {
            var pendingInvoices = await _context.Invoices
                .Where(i => i.IdStatus == 1) // Assuming status ID 1 is for pending
                .Include(i => i.IdStoreNavigation)
                .Include(i => i.IdWarehouseNavigation)
                .Include(i => i.IdStatusNavigation)
                .Include(i => i.IdDriverNavigation)
                .Include(i => i.InvoiceProducts)
                    .ThenInclude(ip => ip.IdProductNavigation)
                .ToListAsync();

            return Ok(pendingInvoices);
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


            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> CreateInvoice(CreateInvoiceRequest request)
        {
            var store = await _context.Stores.SingleOrDefaultAsync(s => s.IdClient == request.ClientId && s.Name == request.StoreName);
            if (store == null)
            {
                return BadRequest("Invalid client ID or store name.");
            }

            // Validate product IDs
            var productIds = request.Products.Select(p => p.ProductId).ToList();
            var existingProductIds = await _context.Products
                                                   .Where(p => productIds.Contains(p.IdProduct))
                                                   .Select(p => p.IdProduct)
                                                   .ToListAsync();
            if (productIds.Count != existingProductIds.Count)
            {
                var invalidProductIds = productIds.Except(existingProductIds).ToList();
                return BadRequest($"Invalid product IDs: {string.Join(", ", invalidProductIds)}");
            }

            var invoice = new Invoice
            {
                IssueDate = DateTime.Now,
                IdStore = store.IdStore,
                IdStatus = 1,
                InvoiceProducts = request.Products.Select(p => new InvoiceProduct
                {
                    IdProduct = p.ProductId,
                    OrderQuantity = p.Quantity
                }).ToList()
            };

            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();

            var coordinator = await _context.Coordinators.FirstOrDefaultAsync();
            if (coordinator?.IdAccount != null)
            {
                await CreateNotification(coordinator.IdAccount.Value, "New invoice created and pending approval.");
            }

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

            var client = await _context.Clients
                                       .Include(c => c.IdAccountNavigation)
                                       .FirstOrDefaultAsync(c => c.IdClient == invoice.IdStoreNavigation.IdClient);
            if (client?.IdAccount != null)
            {
                var statusDescription = await _context.InvoiceStatuses
                                                      .Where(s => s.IdStatus == request.StatusId)
                                                      .Select(s => s.Description)
                                                      .FirstOrDefaultAsync();
                await CreateNotification(client.IdAccount.Value, $"Your invoice status has been updated to: {statusDescription}");
            }

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
            invoice.IdStatus = 2;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("{id}/approve")]
        public async Task<IActionResult> ApproveInvoice(int id)
        {
            try
            {
                var invoice = await _context.Invoices
                    .Include(i => i.InvoiceProducts)
                    .ThenInclude(ip => ip.IdProductNavigation)
                    .SingleOrDefaultAsync(i => i.IdInvoice == id);

                if (invoice == null)
                {
                    return NotFound();
                }

                invoice.IdStatus = 2; // Approved status ID

                foreach (var invoiceProduct in invoice.InvoiceProducts)
                {
                    var product = await _context.Products.FindAsync(invoiceProduct.IdProduct);
                    if (product == null)
                    {
                        return BadRequest($"Product with ID {invoiceProduct.IdProduct} not found.");
                    }

                    if (product.Quantity < invoiceProduct.OrderQuantity)
                    {
                        return BadRequest($"Insufficient quantity for product {product.NameProduct}. Requested: {invoiceProduct.OrderQuantity}, Available: {product.Quantity}");
                    }

                    product.Quantity -= invoiceProduct.OrderQuantity;
                }

                await _context.SaveChangesAsync();

                var client = await _context.Clients
                    .Include(c => c.IdAccountNavigation)
                    .FirstOrDefaultAsync(c => c.IdClient == invoice.IdStoreNavigation.IdClient);
                if (client?.IdAccount != null)
                {
                    await CreateNotification(client.IdAccount.Value, "Your invoice has been approved.");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error. Please try again later.");
            }
        }

        [HttpPut("{id}/reject")]
        public async Task<IActionResult> RejectInvoice(int id)
        {
            var invoice = await _context.Invoices.FindAsync(id);
            if (invoice == null)
            {
                return NotFound();
            }

            invoice.IdStatus = 3; // Rejected status ID
            await _context.SaveChangesAsync();

            var client = await _context.Clients
                                       .Include(c => c.IdAccountNavigation)
                                       .FirstOrDefaultAsync(c => c.IdClient == invoice.IdStoreNavigation.IdClient);
            if (client?.IdAccount != null)
            {
                await CreateNotification(client.IdAccount.Value, "Your invoice has been rejected.");
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




