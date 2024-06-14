using Microsoft.AspNetCore.Mvc;
using PUNDERO.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace PUNDERO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExportInvoiceController : ControllerBase
    {
        private readonly PunderoContext _context;

        public ExportInvoiceController(PunderoContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ExportInvoiceViewModel>> GetExportInvoice(int id)
        {
            var invoice = await _context.Invoices
                .Where(i => i.IdInvoice == id)
                .Select(i => new ExportInvoiceViewModel
                {
                    IdInvoice = i.IdInvoice,
                    IssueDate = i.IssueDate,
                    StoreName = i.IdStoreNavigation.Name,
                    StoreAddress = i.IdStoreNavigation.Address,
                    StorePhone = "+387-33-555-777", // Mockup br
                    StoreEmail = i.IdStoreNavigation.Name.Replace(" ", "").ToLower() + "@pundero.ba",
                    Products = i.InvoiceProducts.Select(ip => new ExportInvoiceViewModel.InvoiceProductViewModel
                    {
                        NameProduct = ip.IdProductNavigation.NameProduct,
                        OrderQuantity = ip.OrderQuantity,
                        UnitPrice = Math.Round(ip.IdProductNavigation.Price, 2),
                        TotalPrice = Math.Round(ip.OrderQuantity * ip.IdProductNavigation.Price, 2)
                    }).ToList(),
                    Subtotal = Math.Round(i.InvoiceProducts.Sum(ip => ip.OrderQuantity * ip.IdProductNavigation.Price), 2),
                    Tax = 0, // ostavicemo ali se moze dodati neka logika
                    TotalAmount = Math.Round(i.InvoiceProducts.Sum(ip => ip.OrderQuantity * ip.IdProductNavigation.Price), 2),
                    DriverName = i.IdDriverNavigation.IdAccountNavigation.FirstName + " " + i.IdDriverNavigation.IdAccountNavigation.LastName,
                    DriverEmail = i.IdDriverNavigation.IdAccountNavigation.Email,
                    DriverPhone = "+387-61-225-883", // Mockup 
                    Note = i.Note
                })
                .FirstOrDefaultAsync();

            if (invoice == null)
            {
                return NotFound();
            }

            return invoice;
        }
    }
}
