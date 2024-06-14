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
        public async Task<IActionResult> GetAllNotifications()
        {
            var notifications = await _context.Notifications
                .Include(n => n.IdAccountNavigation)
                .Include(n => n.IdInvoiceNavigation)
                .ThenInclude(i => i.IdStatusNavigation)
                .OrderByDescending(n => n.IdNotification)
                .ToListAsync();

            var notificationViewModels = notifications.Select(n => new NotificationViewModel
            {
                IdNotification = n.IdNotification,
                IdAccount = n.IdAccount ?? 0,
                AccountType = n.IdAccountNavigation.Type,
                Message = n.Message,
                Seen = n.Seen,
                CreatedAt = n.CreatedAt,
                IdInvoice = n.IdInvoice,
                InvoiceStatusId = n.IdInvoiceNavigation.IdStatus
            }).ToList();

            return Ok(notificationViewModels);
        }

        // GET: api/Notification/coordinator/{idAccount}
        [HttpGet("coordinator/{idAccount}")]
        public async Task<IActionResult> GetCoordinatorNotifications(int idAccount)
        {
            var notifications = await _context.Notifications
                .Include(n => n.IdAccountNavigation)
                .Include(n => n.IdInvoiceNavigation)
                .ThenInclude(i => i.IdStatusNavigation)
                .Where(n => n.IdAccount == idAccount && n.IdAccountNavigation.Type == 1 &&
                            (n.Seen == false || n.Seen == null) &&
                            (n.IdInvoiceNavigation.IdStatus == 1 || n.IdInvoiceNavigation.IdStatus == 4 || n.IdInvoiceNavigation.IdStatus == 5 || n.IdInvoiceNavigation.IdStatus == 6 || n.IdInvoiceNavigation.IdStatus == 7))
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

            var notificationViewModels = notifications.Select(n => new NotificationViewModel
            {
                IdNotification = n.IdNotification,
                IdAccount = n.IdAccount ?? 0,
                AccountType = n.IdAccountNavigation.Type,
                Message = n.Message,
                Seen = n.Seen,
                CreatedAt = n.CreatedAt,
                IdInvoice = n.IdInvoice,
                InvoiceStatusId = n.IdInvoiceNavigation.IdStatus
            }).ToList();

            return Ok(notificationViewModels);
        }

        // GET: api/Notification/client/{storeName}
        [HttpGet("client/{storeName}")]
        public async Task<IActionResult> GetClientNotifications(string storeName)
        {
            var notifications = await _context.Notifications
                .Include(n => n.IdAccountNavigation)
                .Include(n => n.IdInvoiceNavigation)
                .ThenInclude(i => i.IdStatusNavigation)
                .Where(n => n.IdAccountNavigation.Type == 3 &&
                            (n.Seen == false || n.Seen == null) &&
                            (n.IdInvoiceNavigation.IdStatus == 2 || n.IdInvoiceNavigation.IdStatus == 3 || n.IdInvoiceNavigation.IdStatus == 4 || n.IdInvoiceNavigation.IdStatus == 5) &&
                            n.IdInvoiceNavigation.IdStoreNavigation.Name == storeName &&
                            !n.Message.Contains("has been created and waiting for approval by the coordinator."))
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

            var notificationViewModels = notifications
                .Select(n => new NotificationViewModel
                {
                    IdNotification = n.IdNotification,
                    IdAccount = n.IdAccount ?? 0,
                    AccountType = n.IdAccountNavigation.Type,
                    Message = n.Message,
                    Seen = n.Seen,
                    CreatedAt = n.CreatedAt,
                    IdInvoice = n.IdInvoice,
                    InvoiceStatusId = n.IdInvoiceNavigation.IdStatus
                })
                .ToList();

            return Ok(notificationViewModels);
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

        private async Task CreateNotificationForClient(int clientId, string message, int invoiceId)
        {
            var accountId = await _context.Clients.Where(c => c.IdClient == clientId)
                                                  .Select(c => c.IdAccount)
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

        // PUT: api/Notification/invoiceStatus/{id}]
        [HttpPut("invoiceStatus/{id}")]
        public async Task<IActionResult> UpdateInvoiceStatus(int id, [FromBody] InvoiceStatus status)
        {
            var invoice = await _context.Invoices
                .Include(i => i.IdStoreNavigation)
                .SingleOrDefaultAsync(i => i.IdInvoice == id);

            if (invoice == null)
            {
                return NotFound();
            }

            invoice.IdStatus = status.IdStatus;
            await _context.SaveChangesAsync();

            var statusDescription = await _context.InvoiceStatuses
                .Where(s => s.IdStatus == status.IdStatus)
                .Select(s => s.Description)
                .FirstOrDefaultAsync();

            await CreateNotificationForCoordinators($"Invoice status updated to {statusDescription}.", invoice.IdInvoice);

            var client = await _context.Clients
                .Include(c => c.IdAccountNavigation)
                .FirstOrDefaultAsync(c => c.IdClient == invoice.IdStoreNavigation.IdClient);
            if (client?.IdAccount != null)
            {
                await CreateNotificationForClient(client.IdAccount.Value, $"Your invoice status has been updated to: {statusDescription}", invoice.IdInvoice);
            }

            return NoContent();
        }
    }
}
