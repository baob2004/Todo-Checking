using api.Dtos.Token;

namespace api.Interfaces
{
    public interface IGoogleAuthService
    {
        Task<TokenResponseDto?> LoginAsync(string idToken, string? ipAddress = null);
    }
}