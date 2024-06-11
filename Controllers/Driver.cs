using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PUNDERO.Models;

namespace PUNDERO.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class DriverController : ControllerBase
    {
        private readonly PunderoContext _context;

        public DriverController(PunderoContext context)
        {
            _context = context;
        }

        // GET: api/Driver/GetDrivers
        [HttpGet]
        public IActionResult GetDrivers()
        {
            var drivers = _context.Drivers
                .Include(d => d.IdAccountNavigation)
                .Include(d => d.IdTachographNavigation)
                .Include(d => d.MobileDrivers).ThenInclude(md => md.IdMobileNavigation)
                .Include(d => d.VehicleDrivers).ThenInclude(vd => vd.IdVehicleNavigation)
                .Select(d => new DriverViewModel
                {
                    IdDriver = d.IdDriver,
                    IdAccount = d.IdAccount,
                    FirstName = d.IdAccountNavigation.FirstName,
                    LastName = d.IdAccountNavigation.LastName,
                    Email = d.IdAccountNavigation.Email,
                    LicenseNumber = d.LicenseNumber,
                    LicenseCategory = d.LicenseCategory,
                    TachographLabel = d.IdTachographNavigation.Label,
                    TachographIssueDate = d.IdTachographNavigation.IssueDate,
                    TachographExpiryDate = d.IdTachographNavigation.ExpiryDate,
                    Image = d.IdAccountNavigation.Image
                })
                .ToList();

            return Ok(drivers);
        }

        // GET: api/Driver/GetDriverByIdAccount/5
        [HttpGet("{id}")]
        public IActionResult GetDriverByIdAccount(int id)
        {
            var driver = _context.Drivers
                .Include(d => d.IdAccountNavigation)
                .Include(d => d.IdTachographNavigation)
                .Where(d => d.IdAccount == id)
                .Select(d => new DriverViewModel
                {
                    IdDriver = d.IdDriver,
                    IdAccount = d.IdAccount,
                    FirstName = d.IdAccountNavigation.FirstName,
                    LastName = d.IdAccountNavigation.LastName,
                    Email = d.IdAccountNavigation.Email,
                    Password = d.IdAccountNavigation.Password,
                    LicenseNumber = d.LicenseNumber,
                    LicenseCategory = d.LicenseCategory,
                    TachographLabel = d.IdTachographNavigation.Label,
                    TachographIssueDate = d.IdTachographNavigation.IssueDate,
                    TachographExpiryDate = d.IdTachographNavigation.ExpiryDate,
                    Image = d.IdAccountNavigation.Image
                })
                .FirstOrDefault();

            if (driver == null)
            {
                return NotFound();
            }

            return Ok(driver);
        }

        [HttpPost]
        public async Task<IActionResult> AddDriver([FromForm] DriverViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string imagePath = null;

            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                imagePath = Path.Combine("wwwroot", "images", "profile_images", $"{model.FirstName}{model.LastName}.jpg");
                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    await model.ImageFile.CopyToAsync(stream);
                }
            }

            var account = new Account
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Password = model.Password,
                Type = 2, // Driver type
                Image = imagePath != null ? $"/images/profile_images/{model.FirstName}{model.LastName}.jpg" : null
            };

            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();

            var tachograph = new Tachograph
            {
                Label = model.TachographLabel,
                IssueDate = model.TachographIssueDate,
                ExpiryDate = model.TachographExpiryDate
            };

            _context.Tachographs.Add(tachograph);
            await _context.SaveChangesAsync();

            var driver = new Driver
            {
                IdAccount = account.IdAccount,
                LicenseNumber = model.LicenseNumber,
                LicenseCategory = model.LicenseCategory,
                IdTachograph = tachograph.IdTachograph,
                PrivateMobile = 0
            };

            _context.Drivers.Add(driver);
            await _context.SaveChangesAsync();

            return Ok(new DriverViewModel
            {
                IdDriver = driver.IdDriver,
                IdAccount = account.IdAccount,
                FirstName = account.FirstName,
                LastName = account.LastName,
                Email = account.Email,
                LicenseNumber = driver.LicenseNumber,
                LicenseCategory = driver.LicenseCategory,
                TachographLabel = tachograph.Label,
                TachographIssueDate = tachograph.IssueDate,
                TachographExpiryDate = tachograph.ExpiryDate,
                Image = account.Image
            });
        }


        //// POST: api/Driver/AddDriver
        //[HttpPost]
        //public async Task<IActionResult> AddDriver([FromForm] DriverViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    var account = new Account
        //    {
        //        FirstName = model.FirstName,
        //        LastName = model.LastName,
        //        Email = model.Email,
        //        Password = model.Password,
        //        Type = 2, // Driver type
        //        Image = model.Image
        //    };

        //    _context.Accounts.Add(account);
        //    await _context.SaveChangesAsync();

        //    var tachograph = new Tachograph
        //    {
        //        Label = model.TachographLabel,
        //        IssueDate = model.TachographIssueDate,
        //        ExpiryDate = model.TachographExpiryDate
        //    };

        //    _context.Tachographs.Add(tachograph);
        //    await _context.SaveChangesAsync();

        //    var driver = new Driver
        //    {
        //        IdAccount = account.IdAccount,
        //        LicenseNumber = model.LicenseNumber,
        //        LicenseCategory = model.LicenseCategory,
        //        IdTachograph = tachograph.IdTachograph,
        //        PrivateMobile = 0
        //    };

        //    _context.Drivers.Add(driver);
        //    await _context.SaveChangesAsync();

        //    return Ok(new DriverViewModel
        //    {
        //        IdDriver = driver.IdDriver,
        //        IdAccount = account.IdAccount,
        //        FirstName = account.FirstName,
        //        LastName = account.LastName,
        //        Email = account.Email,
        //        LicenseNumber = driver.LicenseNumber,
        //        LicenseCategory = driver.LicenseCategory,
        //        TachographLabel = tachograph.Label,
        //        TachographIssueDate = tachograph.IssueDate,
        //        TachographExpiryDate = tachograph.ExpiryDate,
        //        Image = account.Image
        //    });
        //}

        [HttpPut("{accountId}")]
        public async Task<IActionResult> UpdateDriver(int accountId, [FromForm] DriverViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.SelectMany(x => x.Value.Errors.Select(e => new { x.Key, e.ErrorMessage })).ToList();
                foreach (var error in errors)
                {
                    Console.WriteLine($"Key: {error.Key}, Error: {error.ErrorMessage}");
                }
                return BadRequest(ModelState);
            }

            var driver = await _context.Drivers.Include(d => d.IdTachographNavigation).FirstOrDefaultAsync(d => d.IdAccount == accountId);
            if (driver == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts.FindAsync(accountId);
            if (account == null)
            {
                return NotFound();
            }

            account.FirstName = model.FirstName;
            account.LastName = model.LastName;
            account.Email = model.Email;

            // Retain existing password if not provided
            if (!string.IsNullOrEmpty(model.Password))
            {
                account.Password = model.Password;
            }

            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                var imagePath = Path.Combine("wwwroot", "images", "profile_images", $"{model.FirstName}{model.LastName}.jpg");
                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    await model.ImageFile.CopyToAsync(stream);
                }
                account.Image = $"/images/profile_images/{model.FirstName}{model.LastName}.jpg";
            }

            // Update optional fields
            if (!string.IsNullOrEmpty(model.LicenseNumber))
            {
                driver.LicenseNumber = model.LicenseNumber;
            }

            if (!string.IsNullOrEmpty(model.LicenseCategory))
            {
                driver.LicenseCategory = model.LicenseCategory;
            }

            if (!string.IsNullOrEmpty(model.TachographLabel))
            {
                if (driver.IdTachographNavigation != null)
                {
                    driver.IdTachographNavigation.Label = model.TachographLabel;
                    driver.IdTachographNavigation.IssueDate = model.TachographIssueDate ?? driver.IdTachographNavigation.IssueDate;
                    driver.IdTachographNavigation.ExpiryDate = model.TachographExpiryDate ?? driver.IdTachographNavigation.ExpiryDate;
                }
                else
                {
                    driver.IdTachographNavigation = new Tachograph
                    {
                        Label = model.TachographLabel,
                        IssueDate = model.TachographIssueDate ?? DateTime.Now,
                        ExpiryDate = model.TachographExpiryDate ?? DateTime.Now.AddYears(1) // Default to 1 year if not provided
                    };
                }
            }

            _context.Entry(account).State = EntityState.Modified;
            _context.Entry(driver).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }



        // DELETE: api/Driver/DeleteDriver/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDriver(int id)
        {
            var driver = await _context.Drivers.FindAsync(id);
            if (driver == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts.FindAsync(driver.IdAccount);
            if (account != null)
            {
                _context.Accounts.Remove(account);
            }

            var tachograph = await _context.Tachographs.FindAsync(driver.IdTachograph);
            if (tachograph != null)
            {
                _context.Tachographs.Remove(tachograph);
            }

            _context.Drivers.Remove(driver);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/Driver/UploadImage
        [HttpPost("UploadImage")]
        public async Task<IActionResult> UploadImage([FromForm] IFormFile file, [FromForm] string fileName)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "profile_images", fileName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return Ok(new { path });
        }
    }
}
