using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PUNDERO.Models;
using System.Linq;
using System.Threading.Tasks;

namespace PUNDERO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly PunderoContext _context;

        public NotificationController(PunderoContext context)
        {
            _context = context;
        }

        // GET: api
        [HttpGet]
        public IActionResult GetAllNotifications()
        {
            var notifications = _context.Notifications.OrderByDescending(wh => wh.IdNotification).ToList();
            return Ok(notifications);
        }

        // GET: api/Notification/coordinator
        [HttpGet("coordinator")]
        public async Task<IActionResult> GetCoordinatorNotifications()
        {
            var notifications = await _context.Notifications
                .Where(n => (n.Seen == false || n.Seen == null) && (n.IdInvoiceNavigation.IdStatus == 1 || n.IdInvoiceNavigation.IdStatus == 6 || n.IdInvoiceNavigation.IdStatus == 7))
                .OrderByDescending(n => n.CreatedAt)
                .Include(n => n.IdInvoiceNavigation)
                .ThenInclude(i => i.IdStatusNavigation)
                .ToListAsync();

            var notificationDTOs = notifications.Select(n => new
            {
                n.IdNotification,
                n.Message,
                n.CreatedAt,
                n.Seen,
                Status = n.IdInvoiceNavigation.IdStatusNavigation.Description,
                InvoiceStatusId = n.IdInvoiceNavigation.IdStatus
            }).ToList();

            return Ok(notificationDTOs);
        }

        // PUT: api/Notification/coordinator/{id}/markAsSeen
        [HttpPut("coordinator/{id}/markAsSeen")]
        public async Task<IActionResult> MarkCoordinatorNotificationAsSeen(int id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification == null)
            {
                return NotFound();
            }

            notification.Seen = true;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/Notification/client/{storeName}
        [HttpGet("client/{storeName}")]
        public async Task<IActionResult> GetClientNotifications(string storeName)
        {
            var notifications = await _context.Notifications
                .Where(n => (n.Seen == false || n.Seen == null) && (n.IdInvoiceNavigation.IdStatus == 2 || n.IdInvoiceNavigation.IdStatus == 3 || n.IdInvoiceNavigation.IdStatus == 5) && n.IdInvoiceNavigation.IdStoreNavigation.Name == storeName)
                .OrderByDescending(n => n.CreatedAt)
                .Include(n => n.IdInvoiceNavigation)
                .ThenInclude(i => i.IdStatusNavigation)
                .ToListAsync();

            var notificationDTOs = notifications.Select(n => new
            {
                n.IdNotification,
                n.Message,
                n.CreatedAt,
                n.Seen,
                Status = n.IdInvoiceNavigation.IdStatusNavigation.Description,
                InvoiceStatusId = n.IdInvoiceNavigation.IdStatus
            }).ToList();

            return Ok(notificationDTOs);
        }

        // PUT: api/Notification/client/{id}/markAsSeen
        [HttpPut("client/{id}/markAsSeen")]
        public async Task<IActionResult> MarkClientNotificationAsSeen(int id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification == null)
            {
                return NotFound();
            }

            notification.Seen = true;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // PUT: api/Notification/invoiceStatus/{id}
        [HttpPut("invoiceStatus/{id}")]
        public async Task<IActionResult> UpdateInvoiceStatus(int id, [FromBody] InvoiceStatus status)
        {
            var invoice = await _context.Invoices.FindAsync(id);
            if (invoice == null)
            {
                return NotFound();
            }

            invoice.IdStatus = status.IdStatus;
            await _context.SaveChangesAsync();

            var client = await _context.Clients
                .Include(c => c.IdAccountNavigation)
                .FirstOrDefaultAsync(c => c.IdClient == invoice.IdStoreNavigation.IdClient);

            if (client?.IdAccount != null)
            {
                var message = status.Description;
                var notification = new Notification
                {
                    IdAccount = client.IdAccount.Value,
                    IdInvoice = invoice.IdInvoice,
                    Message = message,
                    Seen = false,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Notifications.Add(notification);
                await _context.SaveChangesAsync();
            }

            return NoContent();
        }
    }
}
