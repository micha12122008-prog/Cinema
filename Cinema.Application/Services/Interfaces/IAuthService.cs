using Cinema.Application.DTO.Auth;

namespace Cinema.Application.Services.Interfaces;

public interface IAuthService
{
    Task RegisterAsync(RegisterRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
}