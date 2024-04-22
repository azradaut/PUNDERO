//    MORAMO U DB RENAMEATI OVO JER STVARA EROR RIJEČ Object, valjda je razervisana


////using System;
////using System.Collections.Generic;
////using System.Linq;
////using Microsoft.AspNetCore.Mvc;
////using Microsoft.EntityFrameworkCore;
////using PUNDERO.Models;

////namespace PUNDERO.Controllers
////{
////    [Route("api/[controller]/[action]")]
////    [ApiController]
////    public class ObjectController : ControllerBase
////    {
////        private readonly PunderoContext _context;

////        public ObjectController(PunderoContext context)
////        {
////            _context = context;
////        }

////        // GET: api/Object
////        [HttpGet]
////        public IActionResult GetObjects()
////        {
////            var objects = _context.Objects.OrderByDescending(o => o.Name).ToList();
////            return Ok(objects);
////        }

////        // GET: api/Object/my-object-name
////        [HttpGet("{name}")]
////        public IActionResult GetObject(string name)
////        {
////            var obj = _context.Objects.FirstOrDefault(o => o.Name == name);

////            if (obj == null)
////            {
////                return NotFound();
////            }

////            return Ok(obj);
////        }

////        // POST: api/Object
////        [HttpPost]
////        public IActionResult PostObject([FromBody] Object obj)
////        {
////            if (!ModelState.IsValid)
////            {
////                return BadRequest(ModelState);
////            }

////            _context.Objects.Add(obj);
////            _context.SaveChanges();

////            return CreatedAtRoute("GetObject", new { name = obj.Name }, obj);
////        }

////        // PUT: api/Object/my-object-name
////        [HttpPut("{name}")]
////        public IActionResult PutObject(string name, [FromBody] Object obj)
////        {
////            if (!ModelState.IsValid)
////            {
////                return BadRequest(ModelState);
////            }

////            if (name != obj.Name)
////            {
////                return BadRequest();
////            }

////            _context.Entry(obj).State = EntityState.Modified;
////            _context.SaveChanges();

////            return NoContent();
////        }

////        // DELETE: api/Object/my-object-name
////        [HttpDelete("{name}")]
////        public IActionResult DeleteObject(string name)
////        {
////            var obj = _context.Objects.FirstOrDefault(o => o.Name == name);
////            if (obj == null)
////            {
////                return NotFound();
////            }

////            _context.Objects.Remove(obj);
////            _context.SaveChanges();

////            return Ok(obj);
////        }
////    }
////}
