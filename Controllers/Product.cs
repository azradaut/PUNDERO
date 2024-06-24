using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PUNDERO.Models;
using System.Threading.Tasks;

namespace PUNDERO.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly PunderoContext _context;

        public ProductController(PunderoContext context)
        {
            _context = context;
        }

        [HttpGet("GetProducts")]
        public async Task<IActionResult> GetProductsByName()
        {
            var products = await _context.Products.Select(p => new { p.IdProduct, p.NameProduct }).ToListAsync();
            return Ok(products);
        }

        // GET: api/Product
        [HttpGet]
        public IActionResult GetProducts()
        {
            var products = _context.Products.OrderByDescending(p => p.NameProduct).ToList();
            return Ok(products);
        }

        // GET: api/Product/my-product-name
        [HttpGet("{name}")]
        public IActionResult GetProduct(string name)
        {
            var product = _context.Products.FirstOrDefault(p => p.NameProduct == name);

            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        // POST: api/Product
        [HttpPost]
        public IActionResult PostProduct([FromBody] Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Products.Add(product);
            _context.SaveChanges();

            return CreatedAtRoute("GetProduct", new { phoneNumber = product.NameProduct }, product);
        }

        // PUT: api/Product/my-product-name
        [HttpPut("{id}")]
        public IActionResult PutProduct(int id, [FromBody] Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != product.IdProduct)
            {
                return BadRequest();
            }

            _context.Entry(product).State = EntityState.Modified;
            _context.SaveChanges();

            return NoContent();
        }

        [HttpPost("")]
        public async Task<IActionResult> CheckAvailability([FromBody] List<ProductRequest> productRequests)
        {
            var availabilityDetails = new List<ProductAvailabilityResponse>();

            foreach (var productRequest in productRequests)
            {
                var product = await _context.Products.SingleOrDefaultAsync(p => p.IdProduct == productRequest.ProductId);
                if (product == null)
                {
                    return BadRequest($"Product with ID {productRequest.ProductId} not found.");
                }

                availabilityDetails.Add(new ProductAvailabilityResponse
                {
                    IdProduct = product.IdProduct,
                    NameProduct = product.NameProduct,
                    OrderQuantity = productRequest.Quantity,
                    AvailableQuantity = product.Quantity,
                    Barcode = product.Barcode
                });
            }

            bool allAvailable = availabilityDetails.All(p => p.AvailableQuantity >= p.OrderQuantity);

            return Ok(new
            {
                AllAvailable = allAvailable,
                Products = availabilityDetails
            });
        }


        public class ProductAvailabilityResponse
        {
            public int IdProduct { get; set; }
            public string NameProduct { get; set; }
            public int OrderQuantity { get; set; }
            public int AvailableQuantity { get; set; }
            public int Barcode { get; set; }
        }

        // DELETE: api/Products/id
        [HttpDelete("{id}")]
        public IActionResult DeleteProduct(int id)
        {
            var product = _context.Products.FirstOrDefault(p => p.IdProduct == id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            _context.SaveChanges();

            return Ok(product);
        }
    }
}