using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Token;
using api.Entities;
using api.Interfaces;
using api.Options;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace api.Services
{
    public class GoogleAuthService : IGoogleAuthService

    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ITokenService _tokenSvc;
        private readonly GoogleAuthOptions _opt;
        public GoogleAuthService(
            UserManager<AppUser> userManager,
            ITokenService tokenSvc,
            IOptions<GoogleAuthOptions> opt)
        {
            _userManager = userManager;
            _tokenSvc = tokenSvc;
            _opt = opt.Value;
        }
        public async Task<TokenResponseDto?> LoginAsync(string idToken, string? ipAddress = null)
        {
            if (string.IsNullOrWhiteSpace(idToken)) return null;

            // 1) Validate Google ID token
            GoogleJsonWebSignature.Payload payload;
            try
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { _opt.ClientId }
                };
                payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
            }
            catch
            {
                return null; // invalid token
            }

            var email = payload.Email;
            if (string.IsNullOrWhiteSpace(email)) return null;

            // 2) Find or create local user
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user is null)
            {
                user = new AppUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true
                };
                var create = await _userManager.CreateAsync(user);
                if (!create.Succeeded) return null;
                await _userManager.AddToRoleAsync(user, "User");
            }

            // 3) Issue access + refresh (simple refresh stored on user)
            var access = _tokenSvc.CreateToken(user);
            var refresh = _tokenSvc.CreateRefreshToken();
            user.RefreshToken = refresh;
            user.RefreshTokenExpiryTime = _tokenSvc.GetRefreshTokenExpiry();
            await _userManager.UpdateAsync(user);

            return new TokenResponseDto
            {
                AccessToken = access,
                RefreshToken = refresh
            };
        }
    }
}