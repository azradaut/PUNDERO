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

        private async Task CreateNotificationForCoordinators(string message, int invoiceId)
        {
            var coordinatorAccounts = await _context.Coordinators.Select(c => c.IdAccount).ToListAsync();
            foreach (var accountId in coordinatorAccounts)
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
            }
            await _context.SaveChangesAsync();
        }

        private async Task CreateNotificationForClient(int accountId, string message, int invoiceId)
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
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a notification for client account ID {AccountId}, Invoice ID {InvoiceId}", accountId, invoiceId);
            }
        }

        private string GetStatusMessageForCoordinator(int statusId, int invoiceId)
        {
            switch (statusId)
            {
                case 1: return $"The invoice no. {invoiceId} has been created and waiting for approval by the coordinator.";
                case 4: return $"The driver is delivering the products for invoice no. {invoiceId}.";
                case 5: return $"The driver has delivered the products from invoice no. {invoiceId} to the client.";
                case 6: return $"The client has confirmed the delivery of invoice no. {invoiceId}.";
                case 7: return $"The client has reported an issue with the delivery of invoice no. {invoiceId}.";
                default: return null;
            }
        }

        private string GetStatusMessageForClient(int statusId, int invoiceId)
        {
            switch (statusId)
            {
                case 2: return $"The invoice no. {invoiceId} has been approved by the coordinator.";
                case 3: return $"The invoice no. {invoiceId} has been rejected by the coordinator.";
                case 4: return $"The driver is delivering the products for invoice no. {invoiceId}.";
                case 5: return $"The driver has delivered the products from invoice no. {invoiceId} to the client.";
                default: return null;
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
                        StatusName = i.IdStatusNavigation.Name,
                        i.Note
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
                        StatusName = i.IdStatusNavigation.Name,
                        i.Note
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
            try
            {
                var pendingInvoices = await _context.Invoices
                    .Where(i => i.IdStatus == 1) // Assuming status ID 1 is for pending
                    .Include(i => i.IdStoreNavigation)
                    .Include(i => i.IdWarehouseNavigation)
                    .Include(i => i.IdStatusNavigation)
                    .Include(i => i.IdDriverNavigation)
                    .Include(i => i.InvoiceProducts)
                        .ThenInclude(ip => ip.IdProductNavigation)
                    .Select(i => new {
                        i.IdInvoice,
                        i.IssueDate,
                        StoreName = i.IdStoreNavigation.Name,
                        WarehouseName = i.IdWarehouseNavigation.NameWarehouse,
                        DriverName = i.IdDriverNavigation.IdAccountNavigation.FirstName + " " + i.IdDriverNavigation.IdAccountNavigation.LastName,
                        Products = i.InvoiceProducts.Select(ip => new {
                            ip.IdProductNavigation.IdProduct,
                            ip.IdProductNavigation.NameProduct,
                            ip.OrderQuantity
                        }).ToList(),
                        i.Note
                    })
                    .ToListAsync();

                return Ok(pendingInvoices);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching pending invoices.");
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
                        TotalAmount = i.InvoiceProducts.Sum(ip => ip.OrderQuantity * ip.IdProductNavigation.Price),
                        i.Note
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
                .Select(i => new {
                    i.IdInvoice,
                    i.IssueDate,
                    StoreName = i.IdStoreNavigation.Name,
                    DriverName = i.IdDriverNavigation != null ? $"{i.IdDriverNavigation.IdAccountNavigation.FirstName} {i.IdDriverNavigation.IdAccountNavigation.LastName}" : null,
                    StatusName = i.IdStatusNavigation.Name,
                    Products = i.InvoiceProducts.Select(ip => new {
                        ip.IdProductNavigation.NameProduct,
                        ip.OrderQuantity,
                        ip.IdProductNavigation.Price,
                        TotalPrice = ip.OrderQuantity * ip.IdProductNavigation.Price
                    }),
                    TotalAmount = i.InvoiceProducts.Sum(ip => ip.OrderQuantity * ip.IdProductNavigation.Price),
                    i.Note
                })
                .SingleOrDefaultAsync(i => i.IdInvoice == id);

            if (invoice == null)
            {
                return NotFound();
            }

            return Ok(invoice);
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
                        StatusName = i.IdStatusNavigation.Name,
                        i.Note
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
            var invoice = await _context.Invoices
                .Include(i => i.IdStoreNavigation)
                .SingleOrDefaultAsync(i => i.IdInvoice == id);
            if (invoice == null)
            {
                return NotFound();
            }

            invoice.IdStatus = 6; // Completed status ID
            await _context.SaveChangesAsync();

            var messageForCoordinator = GetStatusMessageForCoordinator(invoice.IdStatus.Value, invoice.IdInvoice);
            if (messageForCoordinator != null)
            {
                await CreateNotificationForCoordinators(messageForCoordinator, invoice.IdInvoice);
            }

            var client = await _context.Clients
                .Include(c => c.IdAccountNavigation)
                .FirstOrDefaultAsync(c => c.IdClient == invoice.IdStoreNavigation.IdClient);
            if (client?.IdAccount != null)
            {
                var messageForClient = GetStatusMessageForClient(invoice.IdStatus.Value, invoice.IdInvoice);
                if (messageForClient != null)
                {
                    await CreateNotificationForClient(client.IdAccount.Value, messageForClient, invoice.IdInvoice);
                }
            }

            return NoContent();
        }

        [HttpPut("{id}/fail")]
        public async Task<IActionResult> FailInvoice(int id)
        {
            var invoice = await _context.Invoices
                .Include(i => i.IdStoreNavigation)
                .SingleOrDefaultAsync(i => i.IdInvoice == id);
            if (invoice == null)
            {
                return NotFound();
            }

            invoice.IdStatus = 7; // Failed status ID
            await _context.SaveChangesAsync();

            var messageForCoordinator = GetStatusMessageForCoordinator(invoice.IdStatus.Value, invoice.IdInvoice);
            if (messageForCoordinator != null)
            {
                await CreateNotificationForCoordinators(messageForCoordinator, invoice.IdInvoice);
            }

            var client = await _context.Clients
                .Include(c => c.IdAccountNavigation)
                .FirstOrDefaultAsync(c => c.IdClient == invoice.IdStoreNavigation.IdClient);
            if (client?.IdAccount != null)
            {
                var messageForClient = GetStatusMessageForClient(invoice.IdStatus.Value, invoice.IdInvoice);
                if (messageForClient != null)
                {
                    await CreateNotificationForClient(client.IdAccount.Value, messageForClient, invoice.IdInvoice);
                }
            }

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

            var messageForCoordinator = GetStatusMessageForCoordinator(invoice.IdStatus.Value, invoice.IdInvoice);
            if (messageForCoordinator != null)
            {
                await CreateNotificationForCoordinators(messageForCoordinator, invoice.IdInvoice);
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

            var messageForCoordinator = GetStatusMessageForCoordinator(invoice.IdStatus.Value, invoice.IdInvoice);
            if (messageForCoordinator != null)
            {
                await CreateNotificationForCoordinators(messageForCoordinator, invoice.IdInvoice);
            }

            var client = await _context.Clients
                .Include(c => c.IdAccountNavigation)
                .FirstOrDefaultAsync(c => c.IdClient == invoice.IdStoreNavigation.IdClient);
            if (client?.IdAccount != null)
            {
                var messageForClient = GetStatusMessageForClient(invoice.IdStatus.Value, invoice.IdInvoice);
                if (messageForClient != null)
                {
                    await CreateNotificationForClient(client.IdAccount.Value, messageForClient, invoice.IdInvoice);
                }
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
        public async Task<IActionResult> ApproveInvoice(int id, [FromBody] NoteRequest request)
        {
            try
            {
                var invoice = await _context.Invoices
                    .Include(i => i.InvoiceProducts)
                    .ThenInclude(ip => ip.IdProductNavigation)
                    .Include(i => i.IdStoreNavigation) // Ensure IdStoreNavigation is included
                    .SingleOrDefaultAsync(i => i.IdInvoice == id);

                if (invoice == null)
                {
                    return NotFound();
                }

                invoice.IdStatus = 2;
                invoice.Note = request.Note;

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
                    var messageForClient = GetStatusMessageForClient(invoice.IdStatus.Value, invoice.IdInvoice);
                    if (messageForClient != null)
                    {
                        await CreateNotificationForClient(client.IdAccount.Value, messageForClient, invoice.IdInvoice);
                    }
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while approving the invoice.");
                return StatusCode(500, "Internal server error. Please try again later.");
            }
        }


        [HttpPut("{id}/reject")]
        public async Task<IActionResult> RejectInvoice(int id, [FromBody] NoteRequest request)
        {
            Invoice invoice = null;
            try
            {
                invoice = await _context.Invoices
                    .Include(i => i.IdStoreNavigation)
                    .SingleOrDefaultAsync(i => i.IdInvoice == id);

                if (invoice == null)
                {
                    return NotFound();
                }

                invoice.IdStatus = 3; // Rejected 
                invoice.Note = request.Note;
                await _context.SaveChangesAsync();

                var client = await _context.Clients
                    .Include(c => c.IdAccountNavigation)
                    .FirstOrDefaultAsync(c => c.IdClient == invoice.IdStoreNavigation.IdClient);

                if (client?.IdAccount != null)
                {
                    var messageForClient = GetStatusMessageForClient(invoice.IdStatus.Value, invoice.IdInvoice);
                    if (messageForClient != null)
                    {
                        await CreateNotificationForClient(client.IdAccount.Value, messageForClient, invoice.IdInvoice);
                    }
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while rejecting the invoice. Invoice ID: {InvoiceId}, Client ID: {ClientId}", id, invoice?.IdStoreNavigation?.IdClient);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }




        [HttpGet("intransitToClient/{storeName}")]
        public async Task<IActionResult> GetInTransitInvoicesForClient(string storeName)
        {
            try
            {
                var inTransitInvoices = await _context.Invoices
                    .Include(i => i.IdStoreNavigation)
                    .Include(i => i.IdStatusNavigation)
                    .Include(i => i.IdDriverNavigation)
                        .ThenInclude(d => d.IdAccountNavigation)
                    .Include(i => i.InvoiceProducts)
                        .ThenInclude(ip => ip.IdProductNavigation)
                    .Where(i => i.IdStoreNavigation.Name == storeName && i.IdStatus == 4) // Assuming status ID 4 is for in-transit
                    .Select(i => new {
                        DriverId = i.IdDriver,
                        FirstName = i.IdDriverNavigation.IdAccountNavigation.FirstName,
                        LastName = i.IdDriverNavigation.IdAccountNavigation.LastName,
                        LkLatitude = i.IdDriverNavigation.MobileDrivers.Select(md => md.IdMobileNavigation.LkLatitude).FirstOrDefault(),
                        LkLongitude = i.IdDriverNavigation.MobileDrivers.Select(md => md.IdMobileNavigation.LkLongitude).FirstOrDefault(),
                        Invoices = i.IdDriverNavigation.Invoices.Where(inv => inv.IdStatus == 4).Select(inv => inv.IdInvoice)
                    })
                    .ToListAsync();

                return Ok(inTransitInvoices);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching in-transit invoices for the store.");
                return StatusCode(500, "Internal server error");
            }
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

    public class NoteRequest
    {
        public string Note { get; set; }
    }
}
