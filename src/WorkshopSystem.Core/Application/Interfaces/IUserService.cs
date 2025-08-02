using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using WorkshopSystem.Core.Application.DTOs;

namespace WorkshopSystem.Core.Application.Interfaces
{
    public interface IUserService
    {
        Task<IdentityResult> RegisterUserAsync(RegisterUserDto registerDto);
        Task<SignInResult> LoginUserAsync(LoginUserDto loginDto);
        Task LogoutUserAsync();
        Task<UserDto> GetUserByIdAsync(string id);
        Task<UserDto> GetUserByEmailAsync(string email);
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<IEnumerable<UserDto>> GetUsersInRoleAsync(string roleName);
        Task<IdentityResult> AddToRoleAsync(string userId, string roleName);
        Task<IdentityResult> RemoveFromRoleAsync(string userId, string roleName);
        Task<bool> IsInRoleAsync(string userId, string roleName);
        Task<IList<string>> GetUserRolesAsync(string userId);
        Task<IdentityResult> UpdateUserAsync(UserDto userDto);
        Task<IdentityResult> DeleteUserAsync(string userId);
        Task<UserDto> GetCurrentUserAsync(ClaimsPrincipal user);
    }
}
