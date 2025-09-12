using api.Entities;

namespace api.Interfaces
{
    public interface ITokenService
    {
        public string CreateToken(AppUser user);
        public string CreateRefreshToken();
        public DateTime GetRefreshTokenExpiry();
    }
}