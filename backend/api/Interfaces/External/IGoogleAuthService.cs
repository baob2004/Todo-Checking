using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Token;
using Google.Apis.Auth.OAuth2.Responses;

namespace api.Interfaces
{
    public interface IGoogleAuthService
    {
        Task<TokenResponseDto?> LoginAsync(string idToken, string? ipAddress = null);
    }
}