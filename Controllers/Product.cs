using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PUNDERO.Models;

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
        [HttpPut("{name}")]
        public IActionResult PutProduct(string name, [FromBody] Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (name != product.NameProduct)
            {
                return BadRequest();
            }

            _context.Entry(product).State = EntityState.Modified;
            _context.SaveChanges();

            return NoContent();
        }

        // DELETE: api/Product/my-product-name
        [HttpDelete("{name}")]
        public IActionResult DeleteProduct(string name)
        {
            var product = _context.Products.FirstOrDefault(p => p.NameProduct == name);
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
