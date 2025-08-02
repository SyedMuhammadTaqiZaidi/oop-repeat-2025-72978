using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WorkshopSystem.Core.Application.DTOs;
using WorkshopSystem.Core.Application.Interfaces;
using WorkshopSystem.Core.Domain.Entities;

namespace WorkshopSystem.Infrastructure.Identity
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        public async Task<IdentityResult> RegisterUserAsync(RegisterUserDto registerDto)
        {
            var user = new ApplicationUser
            {
                UserName = registerDto.Email,
                Email = registerDto.Email,
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                EmailConfirmed = true 
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (result.Succeeded && !string.IsNullOrEmpty(registerDto.Role))
            {
                await _userManager.AddToRoleAsync(user, registerDto.Role);
            }

            return result;
        }

        public async Task<SignInResult> LoginUserAsync(LoginUserDto loginDto)
        {
            return await _signInManager.PasswordSignInAsync(
                loginDto.Email, 
                loginDto.Password, 
                loginDto.RememberMe, 
                lockoutOnFailure: false);
        }

        public async Task LogoutUserAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<UserDto> GetUserByIdAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            return await MapToUserDto(user);
        }

        public async Task<UserDto> GetUserByEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return await MapToUserDto(user);
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            var userDtos = new List<UserDto>();
            
            foreach (var user in users)
            {
                var userDto = await MapToUserDto(user);
                userDtos.Add(userDto);
            }
            
            return userDtos;
        }

        public async Task<IEnumerable<UserDto>> GetUsersInRoleAsync(string roleName)
        {
            var users = await _userManager.GetUsersInRoleAsync(roleName);
            var userDtos = new List<UserDto>();
            
            foreach (var user in users)
            {
                var userDto = await MapToUserDto(user);
                userDtos.Add(userDto);
            }
            
            return userDtos;
        }

        public async Task<IdentityResult> AddToRoleAsync(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException($"User with ID {userId} not found.");

            return await _userManager.AddToRoleAsync(user, roleName);
        }

        public async Task<IdentityResult> RemoveFromRoleAsync(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException($"User with ID {userId} not found.");

            return await _userManager.RemoveFromRoleAsync(user, roleName);
        }

        public async Task<bool> IsInRoleAsync(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException($"User with ID {userId} not found.");

            return await _userManager.IsInRoleAsync(user, roleName);
        }

        public async Task<IList<string>> GetUserRolesAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException($"User with ID {userId} not found.");

            return await _userManager.GetRolesAsync(user);
        }

        public async Task<IdentityResult> UpdateUserAsync(UserDto userDto)
        {
            var user = await _userManager.FindByIdAsync(userDto.Id);
            if (user == null)
                throw new KeyNotFoundException($"User with ID {userDto.Id} not found.");

            user.FirstName = userDto.FirstName;
            user.LastName = userDto.LastName;
            user.Email = userDto.Email;
            user.PhoneNumber = userDto.PhoneNumber;
            user.UserName = userDto.Email;

            return await _userManager.UpdateAsync(user);
        }

        public async Task<IdentityResult> DeleteUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException($"User with ID {userId} not found.");

            return await _userManager.DeleteAsync(user);
        }

        public async Task<UserDto> GetCurrentUserAsync(ClaimsPrincipal user)
        {
            var currentUser = await _userManager.GetUserAsync(user);
            if (currentUser == null)
                return null;

            return await MapToUserDto(currentUser);
        }

        private async Task<UserDto> MapToUserDto(ApplicationUser user)
        {
            if (user == null)
                return null;

            var roles = await _userManager.GetRolesAsync(user);
            
            return new UserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                EmailConfirmed = user.EmailConfirmed,
                Roles = roles.ToList()
            };
        }
    }
}
