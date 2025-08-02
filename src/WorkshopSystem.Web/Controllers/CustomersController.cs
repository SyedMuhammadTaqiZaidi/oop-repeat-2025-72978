using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkshopSystem.Core.Application.DTOs;
using WorkshopSystem.Core.Application.Interfaces;

namespace WorkshopSystem.Web.Controllers
{
    [Authorize]
    public class CustomersController : Controller
    {
        private readonly IUserService _userService;

        public CustomersController(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            var customers = await _userService.GetUsersInRoleAsync("Customer");
            return View(customers);
        }

        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var customer = await _userService.GetUserByIdAsync(id);
            if (customer == null) return NotFound();

            return View(customer);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(CreateUserDto userDto)
        {
            userDto.Role = "Customer";

            if (!ModelState.IsValid)
            {
                return View(userDto);
            }

            try
            {
                var registerDto = new RegisterUserDto
                {
                    Email = userDto.Email,
                    FirstName = userDto.FirstName,
                    LastName = userDto.LastName,
                    Password = userDto.Password,
                    ConfirmPassword = userDto.ConfirmPassword,
                    Role = userDto.Role
                };

                var result = await _userService.RegisterUserAsync(registerDto);
                if (result.Succeeded)
                {
                    var user = await _userService.GetUserByEmailAsync(userDto.Email);
                    if (user != null)
                    {
                        var roleResult = await _userService.AddToRoleAsync(user.Id, "Customer");
                        if (!roleResult.Succeeded)
                        {
                            foreach (var error in roleResult.Errors)
                            {
                                ModelState.AddModelError("", $"Role assignment failed: {error.Description}");
                            }
                            return View(userDto);
                        }
                    }
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error creating customer: {ex.Message}");
            }

            return View(userDto);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var customer = await _userService.GetUserByIdAsync(id);
            if (customer == null) return NotFound();

            return View(customer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(string id, UserDto userDto)
        {
            if (id != userDto.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                return View(userDto);
            }

            var result = await _userService.UpdateUserAsync(userDto);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(userDto);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var customer = await _userService.GetUserByIdAsync(id);
            if (customer == null) return NotFound();

            return View(customer);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var result = await _userService.DeleteUserAsync(id);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            var customer = await _userService.GetUserByIdAsync(id);
            return View("Delete", customer);
        }

        [AllowAnonymous]
        public async Task<IActionResult> DebugCustomers()
        {
            var allUsers = await _userService.GetAllUsersAsync();
            var customers = await _userService.GetUsersInRoleAsync("Customer");
            
            var debugInfo = new
            {
                TotalUsers = allUsers.Count(),
                TotalCustomers = customers.Count(),
                AllUsers = allUsers.Select(u => new { u.Id, u.Email, u.FirstName, u.LastName, u.Roles }),
                Customers = customers.Select(c => new { c.Id, c.Email, c.FirstName, c.LastName, c.Roles })
            };
            
            return Json(debugInfo);
        }
    }
} 