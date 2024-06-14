//using System;
//using System.Collections.Generic;
//using System.Linq;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;

//using PUNDERO.Models;

//namespace PUNDERO.Controllers
//{
//    [Route("api/[controller]/[action]")]
//    [ApiController]
//    public class InvoiceController : ControllerBase
//    {
//        private readonly PunderoContext _context;

//        public InvoiceController(PunderoContext context)
//        {
//            _context = context;
//        }
//        public class InvoiceDto
//        {
//            public int IdInvoice { get; set; }
//            public int? IdDriver { get; set; }
//            public int? IdStatus { get; set; }
//            public string StoreName { get; set; }
//            public string WarehouseName { get; set; }

//            public List<ProductDto> Products { get; set; }
//            public string Product { get; set; }
//            public DateTime IssueDate { get; set; }
//            public double StoreLatitude { get; set; }
//            public double StoreLongitude { get; set; }
//        }

//        public class ProductDto
//        {
//            public string ProductName { get; set; }
//            public int Quantity { get; set; }
//        }



//        // GET: api/Invoice
//        [HttpGet]
//        public IActionResult GetInvoices()
//        {
//            var invoices = _context.Invoices.ToList();
//            return Ok(invoices);
//        }

//        // GET: api/Invoice/
//        [HttpGet("{id}")]
//        public IActionResult GetInvoice(int id)
//        {
//            var invoice = _context.Invoices.FirstOrDefault(v => v.IdInvoice == id);

//            if (invoice == null)
//            {
//                return NotFound();
//            }

//            return Ok(invoice);
//        }
//        // POST: api/Invoice
//        [HttpPost]
//        public IActionResult PostInvoice([FromBody] Invoice invoice)
//        {
//            if (!ModelState.IsValid)
//            {
//                return BadRequest(ModelState);
//            }

//            _context.Invoices.Add(invoice);
//            _context.SaveChanges();

//            return CreatedAtRoute("GetInvoice", new { id = invoice.IdInvoice }, invoice);
//        }

//        // PUT: api/Invoice/1
//        [HttpPut("{id}")]
//        public IActionResult PutInvoice(int id, [FromBody] Invoice invoice)
//        {
//            if (!ModelState.IsValid)
//            {
//                return BadRequest(ModelState);
//            }

//            if (id != invoice.IdInvoice)
//            {
//                return BadRequest();
//            }

//            _context.Entry(invoice).State = EntityState.Modified;
//            _context.SaveChanges();

//            return NoContent();
//        }

//        // DELETE: api/Invoice/1
//        [HttpDelete("{id}")]
//        public IActionResult DeleteInvoice(int id)
//        {
//            var invoice = _context.Invoices.FirstOrDefault(i => i.IdInvoice == id);
//            if (invoice == null)
//            {
//                return NotFound();
//            }

//            _context.Invoices.Remove(invoice);
//            _context.SaveChanges();

//            return Ok(invoice);
//        }
//        [HttpGet("{driverId}")]

//        public IActionResult GetApprovedInvoicesForDriver(int driverId)
//        {
//            var invoices = _context.Invoices
//       .Include(i => i.IdStatusNavigation)
//       .Include(i => i.IdStoreNavigation)
//       .Include(i => i.IdWarehouseNavigation)
//       .Include(i => i.InvoiceProducts)
//           .ThenInclude(ip => ip.IdProductNavigation)
//       .Where(i => i.IdDriver == driverId && i.IdStatus == 2) // Only approved invoices
//       .Select(i => new InvoiceDto
//       {
//           IdInvoice = i.IdInvoice,
//           IdDriver = i.IdDriver,
//           IdStatus = i.IdStatus,
//           StoreName = i.IdStoreNavigation.Name,
//           WarehouseName = i.IdWarehouseNavigation.NameWarehouse,
//           IssueDate = i.IssueDate,
//           StoreLatitude = i.IdStoreNavigation.Latitude,
//           StoreLongitude = i.IdStoreNavigation.Longitude,
//           Products = i.InvoiceProducts.Select(ip => new ProductDto
//           {
//               ProductName = ip.IdProductNavigation.NameProduct,
//               Quantity = ip.OrderQuantity
//           }).ToList()
//       })
//       .ToList();

//            if (!invoices.Any())
//            {
//                return NotFound();
//            }

//            return Ok(invoices);
//        }

//        [HttpGet]
//        public IActionResult GetDeliveredInvoicesForClient(int driverId)
//        {
//            var invoices = _context.Invoices
//                .Include(i => i.IdStatusNavigation)
//                .Include(i => i.IdStoreNavigation)
//                .Include(i => i.IdWarehouseNavigation)
//                .Where(i => i.IdDriver == driverId && i.IdStatus == 5) // Only deliovered invoices
//                .Select(i => new InvoiceDto
//                {
//                    IdInvoice = i.IdInvoice,
//                    IdDriver = i.IdDriver,
//                    IdStatus = i.IdStatus,
//                    StoreName = i.IdStoreNavigation.Name,
//                    WarehouseName = i.IdWarehouseNavigation.NameWarehouse,
//                    IssueDate = i.IssueDate
//                })
//                .ToList();

//            if (!invoices.Any())
//            {
//                return NotFound();
//            }

//            return Ok(invoices);
//        }
//        [HttpGet("{driverId}")]
//        public IActionResult GetInTransitInvoicesForDriver(int driverId)
//        {
//            var invoices = _context.Invoices
//         .Include(i => i.IdStatusNavigation)
//         .Include(i => i.IdStoreNavigation)
//         .Include(i => i.IdWarehouseNavigation)
//         .Include(i => i.InvoiceProducts) // Include InvoiceProducts
//         .ThenInclude(ip => ip.IdProductNavigation) // Include Product details
//         .Where(i => i.IdDriver == driverId && i.IdStatus == 4) // Only In Transit invoices
//         .Select(i => new InvoiceDto
//         {
//             IdInvoice = i.IdInvoice,
//             IdDriver = i.IdDriver,
//             IdStatus = i.IdStatus,
//             StoreName = i.IdStoreNavigation.Name,
//             WarehouseName = i.IdWarehouseNavigation.NameWarehouse,
//             IssueDate = i.IssueDate,
//             StoreLatitude = i.IdStoreNavigation.Latitude,
//             StoreLongitude = i.IdStoreNavigation.Longitude,
//             Products = i.InvoiceProducts.Select(ip => new ProductDto
//             {
//                 ProductName = ip.IdProductNavigation.NameProduct,
//                 Quantity = ip.OrderQuantity
//             }).ToList()
//         })
//         .ToList();

//            if (!invoices.Any())
//            {
//                return NotFound();
//            }

//            return Ok(invoices);
//        }

//        [HttpPut("{id}/status")]
//        public IActionResult UpdateInvoiceStatus(int id, [FromBody] int statusId)
//        {
//            var invoice = _context.Invoices.FirstOrDefault(i => i.IdInvoice == id);
//            if (invoice == null)
//            {
//                return NotFound();
//            }

//            invoice.IdStatus = statusId;
//            _context.Entry(invoice).State = EntityState.Modified;
//            _context.SaveChanges();

//            return NoContent();
//        }
//    }
//}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public class InvoiceDto
        {
            public int IdInvoice { get; set; }
            public int? IdDriver { get; set; }
            public int? IdStatus { get; set; }
            public string StoreName { get; set; }
            public string WarehouseName { get; set; }
            public List<ProductDto> Products { get; set; }
            public string Product { get; set; }
            public DateTime IssueDate { get; set; }
            public double StoreLatitude { get; set; }
            public double StoreLongitude { get; set; }
        }

        public class ProductDto
        {
            public string ProductName { get; set; }
            public int Quantity { get; set; }
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

        private async Task CreateNotificationForClientByStoreName(string storeName, string message, int invoiceId)
        {
            var accountId = await _context.Stores
                .Where(s => s.Name == storeName)
                .Select(s => s.IdClientNavigation.IdAccount)
                .FirstOrDefaultAsync();

            if (accountId != null)
            {
                var notification = new Notification
                {
                    IdAccount = accountId.Value,
                    Message = message,
                    Seen = false,
                    CreatedAt = DateTime.Now,
                    IdInvoice = invoiceId
                };
                _context.Notifications.Add(notification);
                await _context.SaveChangesAsync();
            }
        }

        // GET: api/Invoice
        [HttpGet]
        public IActionResult GetInvoices()
        {
            var invoices = _context.Invoices.ToList();
            return Ok(invoices);
        }

        // GET: api/Invoice/{id}
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

        // PUT: api/Invoice/{id}
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

        // DELETE: api/Invoice/{id}
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

        [HttpGet("{driverId}")]
        public IActionResult GetApprovedInvoicesForDriver(int driverId)
        {
            var invoices = _context.Invoices
                .Include(i => i.IdStatusNavigation)
                .Include(i => i.IdStoreNavigation)
                .Include(i => i.IdWarehouseNavigation)
                .Include(i => i.InvoiceProducts)
                    .ThenInclude(ip => ip.IdProductNavigation)
                .Where(i => i.IdDriver == driverId && i.IdStatus == 2) // Only approved invoices
                .Select(i => new InvoiceDto
                {
                    IdInvoice = i.IdInvoice,
                    IdDriver = i.IdDriver,
                    IdStatus = i.IdStatus,
                    StoreName = i.IdStoreNavigation.Name,
                    WarehouseName = i.IdWarehouseNavigation.NameWarehouse,
                    IssueDate = i.IssueDate,
                    StoreLatitude = i.IdStoreNavigation.Latitude,
                    StoreLongitude = i.IdStoreNavigation.Longitude,
                    Products = i.InvoiceProducts.Select(ip => new ProductDto
                    {
                        ProductName = ip.IdProductNavigation.NameProduct,
                        Quantity = ip.OrderQuantity
                    }).ToList()
                })
                .ToList();

            if (!invoices.Any())
            {
                return NotFound();
            }

            return Ok(invoices);
        }

        [HttpGet]
        public IActionResult GetDeliveredInvoicesForClient(int driverId)
        {
            var invoices = _context.Invoices
                .Include(i => i.IdStatusNavigation)
                .Include(i => i.IdStoreNavigation)
                .Include(i => i.IdWarehouseNavigation)
                .Where(i => i.IdDriver == driverId && i.IdStatus == 5) // Only delivered invoices
                .Select(i => new InvoiceDto
                {
                    IdInvoice = i.IdInvoice,
                    IdDriver = i.IdDriver,
                    IdStatus = i.IdStatus,
                    StoreName = i.IdStoreNavigation.Name,
                    WarehouseName = i.IdWarehouseNavigation.NameWarehouse,
                    IssueDate = i.IssueDate
                })
                .ToList();

            if (!invoices.Any())
            {
                return NotFound();
            }

            return Ok(invoices);
        }

        [HttpGet("{driverId}")]
        public IActionResult GetInTransitInvoicesForDriver(int driverId)
        {
            var invoices = _context.Invoices
                .Include(i => i.IdStatusNavigation)
                .Include(i => i.IdStoreNavigation)
                .Include(i => i.IdWarehouseNavigation)
                .Include(i => i.InvoiceProducts) // Include InvoiceProducts
                .ThenInclude(ip => ip.IdProductNavigation) // Include Product details
                .Where(i => i.IdDriver == driverId && i.IdStatus == 4) // Only In Transit invoices
                .Select(i => new InvoiceDto
                {
                    IdInvoice = i.IdInvoice,
                    IdDriver = i.IdDriver,
                    IdStatus = i.IdStatus,
                    StoreName = i.IdStoreNavigation.Name,
                    WarehouseName = i.IdWarehouseNavigation.NameWarehouse,
                    IssueDate = i.IssueDate,
                    StoreLatitude = i.IdStoreNavigation.Latitude,
                    StoreLongitude = i.IdStoreNavigation.Longitude,
                    Products = i.InvoiceProducts.Select(ip => new ProductDto
                    {
                        ProductName = ip.IdProductNavigation.NameProduct,
                        Quantity = ip.OrderQuantity
                    }).ToList()
                })
                .ToList();

            if (!invoices.Any())
            {
                return NotFound();
            }

            return Ok(invoices);
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateInvoiceStatus(int id, [FromBody] int statusId)
        {
            var invoice = await _context.Invoices
                .Include(i => i.IdStoreNavigation)
                .SingleOrDefaultAsync(i => i.IdInvoice == id);

            if (invoice == null)
            {
                return NotFound();
            }

            invoice.IdStatus = statusId;
            await _context.SaveChangesAsync();

            var statusDescription = await _context.InvoiceStatuses
                .Where(s => s.IdStatus == statusId)
                .Select(s => s.Description)
                .FirstOrDefaultAsync();

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
                    await CreateNotificationForClientByStoreName(invoice.IdStoreNavigation.Name, messageForClient, invoice.IdInvoice);
                }
            }

            return NoContent();
        }

        private string GetStatusMessageForCoordinator(int statusId, int invoiceId)
        {
            switch (statusId)
            {
                case 1: return $"The invoice no. {invoiceId} has been created and waiting for approval by the coordinator.";
                case 4: return $"The invoice no. {invoiceId} is in transit.";
                case 5: return $"The invoice no. {invoiceId} is delivered.";
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
                case 4: return $"The invoice no. {invoiceId} is in transit.";
                case 5: return $"The invoice no. {invoiceId} is delivered.";
                default: return null;
            }
        }
    }
}

