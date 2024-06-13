using Microsoft.AspNetCore.Mvc;
using PUNDERO.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace PUNDERO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsCoordinatorController : ControllerBase
    {
        private readonly PunderoContext _context;

        public ProductsCoordinatorController(PunderoContext context)
        {
            _context = context;
        }

        // GET: api/ProductsCoordinator/GetProductsCoordinator
        [HttpGet("GetProductsCoordinator")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductsCoordinator()
        {
            return await _context.Products.Include(p => p.IdWarehouseNavigation).ToListAsync();
        }

        // GET: api/ProductsCoordinator/GetProductCoordinator/5
        [HttpGet("GetProductCoordinator/{id}")]
        public async Task<ActionResult<Product>> GetProductCoordinator(int id)
        {
            var product = await _context.Products.Include(p => p.IdWarehouseNavigation).FirstOrDefaultAsync(p => p.IdProduct == id);

            if (product == null)
            {
                return NotFound();
            }

            return product;
        }

        // POST: api/ProductsCoordinator/AddProductCoordinator
        [HttpPost("AddProductCoordinator")]
        public async Task<ActionResult<Product>> AddProductCoordinator(Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProductCoordinator", new { id = product.IdProduct }, product);
        }

        // PUT: api/ProductsCoordinator/UpdateProductCoordinator/5
        [HttpPut("UpdateProductCoordinator/{id}")]
        public async Task<IActionResult> UpdateProductCoordinator(int id, Product product)
        {
            if (id != product.IdProduct)
            {
                return BadRequest();
            }

            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/ProductsCoordinator/DeleteProductCoordinator/5
        [HttpDelete("DeleteProductCoordinator/{id}")]
        public async Task<IActionResult> DeleteProductCoordinator(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.IdProduct == id);
        }
    }
}
