using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkshopSystem.Core.Application.DTOs;
using WorkshopSystem.Core.Application.Interfaces;
using WorkshopSystem.Core.Domain.Enums;

namespace WorkshopSystem.Web.Controllers
{
    [Authorize]
    public class ServiceRecordsController : Controller
    {
        private readonly IServiceRecordService _serviceRecordService;
        private readonly IUserService _userService;

        public ServiceRecordsController(
            IServiceRecordService serviceRecordService,
            IUserService userService)
        {
            _serviceRecordService = serviceRecordService;
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userService.GetCurrentUserAsync(User);
            var isAdmin = await _userService.IsInRoleAsync(currentUser.Id, "Admin");
            var isMechanic = await _userService.IsInRoleAsync(currentUser.Id, "Mechanic");
            var isCustomer = await _userService.IsInRoleAsync(currentUser.Id, "Customer");
            
            var records = isAdmin 
                ? await _serviceRecordService.GetAllServiceRecordsAsync()
                : isMechanic
                    ? await _serviceRecordService.GetServiceRecordsByMechanicIdAsync(currentUser.Id)
                    : isCustomer
                        ? await _serviceRecordService.GetServiceRecordsByCustomerIdAsync(currentUser.Id)
                        : await _serviceRecordService.GetServiceRecordsByRequestedByIdAsync(currentUser.Id);
                
            return View(records);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var record = await _serviceRecordService.GetServiceRecordByIdAsync(id.Value);
            if (record == null) return NotFound();

            return View(record);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            var customers = await _userService.GetUsersInRoleAsync("Customer");
            var mechanics = await _userService.GetUsersInRoleAsync("Mechanic");
            
            ViewBag.CustomersCount = customers?.Count() ?? 0;
            ViewBag.MechanicsCount = mechanics?.Count() ?? 0;
            ViewBag.Customers = customers;
            ViewBag.Mechanics = mechanics;
            
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(WorkshopSystem.Web.ViewModels.ServiceRecordCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Customers = await _userService.GetUsersInRoleAsync("Customer");
                ViewBag.Mechanics = await _userService.GetUsersInRoleAsync("Mechanic");
                return View(model);
            }

            var currentUser = await _userService.GetCurrentUserAsync(User);
            if (currentUser == null)
            {
                ModelState.AddModelError("", "User not found");
                ViewBag.Customers = await _userService.GetUsersInRoleAsync("Customer");
                ViewBag.Mechanics = await _userService.GetUsersInRoleAsync("Mechanic");
                return View(model);
            }

            var recordDto = new CreateServiceRecordDto
            {
                CustomerId = model.CustomerId,
                CarId = model.CarId,
                AssignedMechanicId = model.MechanicId,
                Description = "",
                HoursWorked = 0,
                RequestedById = currentUser.Id
            };

            await _serviceRecordService.CreateServiceRecordAsync(recordDto);
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin,Mechanic")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var record = await _serviceRecordService.GetServiceRecordByIdAsync(id.Value);
            if (record == null) return NotFound();

            var currentUser = await _userService.GetCurrentUserAsync(User);
            var isAdmin = await _userService.IsInRoleAsync(currentUser.Id, "Admin");
            var isMechanic = await _userService.IsInRoleAsync(currentUser.Id, "Mechanic");

            if (isMechanic && !isAdmin && record.AssignedMechanicId != currentUser.Id)
            {
                return Forbid();
            }

            var updateDto = new UpdateServiceRecordDto
            {
                Id = record.Id,
                Description = record.Description,
                HoursWorked = record.HoursWorked,
                Status = record.Status,
                AssignedMechanicId = record.AssignedMechanicId
            };

            ViewBag.Mechanics = await _userService.GetUsersInRoleAsync("Mechanic");
            ViewBag.IsAdmin = isAdmin;
            ViewBag.IsMechanic = isMechanic;
            ViewBag.OriginalRecord = record;
            return View(updateDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Mechanic")]
        public async Task<IActionResult> Edit(int id, UpdateServiceRecordDto updateDto)
        {
            if (id != updateDto.Id) return NotFound();

            var record = await _serviceRecordService.GetServiceRecordByIdAsync(id);
            if (record == null) return NotFound();

            var currentUser = await _userService.GetCurrentUserAsync(User);
            var isAdmin = await _userService.IsInRoleAsync(currentUser.Id, "Admin");
            var isMechanic = await _userService.IsInRoleAsync(currentUser.Id, "Mechanic");

            if (isMechanic && !isAdmin && record.AssignedMechanicId != currentUser.Id)
            {
                return Forbid();
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Mechanics = await _userService.GetUsersInRoleAsync("Mechanic");
                ViewBag.IsAdmin = isAdmin;
                ViewBag.IsMechanic = isMechanic;
                return View(updateDto);
            }

            await _serviceRecordService.UpdateServiceRecordAsync(id, updateDto);
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var record = await _serviceRecordService.GetServiceRecordByIdAsync(id.Value);
            if (record == null) return NotFound();

            return View(record);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _serviceRecordService.DeleteServiceRecordAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Mechanic")]
        public async Task<IActionResult> CompleteService(int id)
        {
            var record = await _serviceRecordService.GetServiceRecordByIdAsync(id);
            if (record == null) return NotFound();

            var updateDto = new UpdateServiceRecordDto
            {
                Id = record.Id,
                Description = record.Description,
                HoursWorked = record.HoursWorked,
                Status = ServiceStatus.Completed
            };

            await _serviceRecordService.UpdateServiceRecordAsync(id, updateDto);
            return RedirectToAction(nameof(Index));
        }
    }
}
