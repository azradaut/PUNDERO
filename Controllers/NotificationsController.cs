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

        // GET: api/Notifications
        [HttpGet]
        public async Task<IActionResult> GetNotifications()
        {
            var notifications = await _context.Notifications.ToListAsync();
            return Ok(notifications);
        }

        [HttpGet("{accountId}")]
        public async Task<IActionResult> GetNotifications(int accountId)
        {
            var notifications = await _context.Notifications
                .Where(n => n.IdAccount == accountId && (n.Seen == false || n.Seen == null))
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

            return Ok(notifications);
        }

        [HttpPost]
        public async Task<IActionResult> CreateNotification(Notification notification)
        {
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetNotifications), new { accountId = notification.IdAccount }, notification);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> MarkAsSeen(int id)
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
    }
}
