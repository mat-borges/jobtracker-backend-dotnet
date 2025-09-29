using JobTracker.Application.DTOs;
using JobTracker.Domain.Entities;

namespace JobTracker.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> LoginAsync(UserLoginDto loginDto);
        Task<User> RegisterAsync(UserRegisterDto registerDto);
    }
}
