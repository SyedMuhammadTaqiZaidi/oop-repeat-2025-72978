using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WorkshopSystem.Core.Domain.Entities;
using WorkshopSystem.Infrastructure.Data;
using System.Linq;
using System.Threading.Tasks;

namespace WorkshopSystem.Web.Controllers
{
    public class CarsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CarsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var cars = _context.Cars.Include(c => c.Customer);
            return View(await cars.ToListAsync());
        }

        public IActionResult Create()
        {
            ViewData["CustomerId"] = new SelectList(_context.Users.Where(u => EF.Property<string>(u, "Discriminator") == "Customer").ToList(), "Id", "Email");
            return View(new WorkshopSystem.Web.ViewModels.CarCreateViewModel
            {
                RegistrationNumber = "",
                CustomerId = ""
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(WorkshopSystem.Web.ViewModels.CarCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var car = new Car {
                    RegistrationNumber = model.RegistrationNumber,
                    CustomerId = model.CustomerId
                };
                _context.Add(car);
                try {
                    await _context.SaveChangesAsync();
                    TempData["CarCreateSuccess"] = "Car created successfully.";
                } catch (Exception ex) {
                    TempData["CarCreateError"] = "Error creating car: " + ex.Message;
                    ViewBag.ModelStateErrors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                    ViewData["CustomerId"] = new SelectList(_context.Users.Where(u => EF.Property<string>(u, "Discriminator") == "Customer").ToList(), "Id", "Email", model.CustomerId);
                    return View(model);
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerId"] = new SelectList(_context.Users.Where(u => EF.Property<string>(u, "Discriminator") == "Customer").ToList(), "Id", "Email", model.CustomerId);
            ViewBag.ModelStateErrors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return View(model);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();
            var car = await _context.Cars.FindAsync(id);
            if (car == null)
                return NotFound();
            var model = new WorkshopSystem.Web.ViewModels.CarCreateViewModel {
                RegistrationNumber = car.RegistrationNumber,
                CustomerId = car.CustomerId
            };
            ViewData["CustomerId"] = new SelectList(_context.Users.Where(u => EF.Property<string>(u, "Discriminator") == "Customer").ToList(), "Id", "Email", car.CustomerId);
            ViewBag.ModelStateErrors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            ViewBag.CarId = car.Id;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, WorkshopSystem.Web.ViewModels.CarCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var car = await _context.Cars.FindAsync(id);
                if (car == null)
                    return NotFound();
                car.RegistrationNumber = model.RegistrationNumber;
                car.CustomerId = model.CustomerId;
                try
                {
                    _context.Update(car);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CarExists(id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerId"] = new SelectList(_context.Users.Where(u => EF.Property<string>(u, "Discriminator") == "Customer").ToList(), "Id", "Email", model.CustomerId);
            ViewBag.ModelStateErrors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            ViewBag.CarId = id;
            return View(model);
        }

       
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();
            var car = await _context.Cars.Include(c => c.Customer).FirstOrDefaultAsync(m => m.Id == id);
            if (car == null)
                return NotFound();
           
            ViewBag.ModelStateErrors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return View(car);
        }

    
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var car = await _context.Cars.FindAsync(id);
            if (car == null)
                return NotFound();
                
            _context.Cars.Remove(car);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CarExists(int id)
        {
            return _context.Cars.Any(e => e.Id == id);
        }
        [HttpGet]
        public JsonResult GetCarsByCustomer(string customerId)
        {
            var cars = _context.Cars
                .Where(c => c.CustomerId == customerId)
                .Select(c => new { c.Id, c.RegistrationNumber })
                .ToList();
            return Json(cars);
        }
    }
}
