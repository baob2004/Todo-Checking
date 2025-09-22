using System.Text;
using api.Dtos.ResetPassword;
using api.Dtos.Token;
using api.Dtos.User;
using api.Entities;
using api.Interfaces;
using api.Interfaces.External;
using api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IGoogleAuthService _googleSvc;
        private readonly IEmailSender _emailSender;
        public AuthController(UserManager<AppUser> userManager, ITokenService tokenService, IGoogleAuthService googleSvc, SignInManager<AppUser> signInManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _googleSvc = googleSvc;
            _signInManager = signInManager;
            _emailSender = emailSender;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var user = new AppUser
                {
                    UserName = dto.Username,
                    Email = dto.Email
                };

                if (await _userManager.FindByNameAsync(user.UserName) != null)
                {
                    return BadRequest("Username already taken");
                }
                if (await _userManager.FindByEmailAsync(user.Email) != null)
                {
                    return BadRequest("Email already registed");
                }

                var createdUser = await _userManager.CreateAsync(user, dto.Password);
                if (createdUser.Succeeded)
                {
                    var roleResult = await _userManager.AddToRoleAsync(user, "User");
                    if (roleResult.Succeeded)
                    {
                        return Ok(new NewUserDto
                        {
                            Username = user.UserName,
                            Email = user.Email,
                            Token = _tokenService.CreateToken(user)
                        });
                    }
                    else
                    {
                        return StatusCode(500, roleResult.Errors);
                    }
                }
                else
                {
                    return StatusCode(500, createdUser.Errors);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var user = await _userManager.FindByNameAsync(dto.Username);

                if (user == null) return Unauthorized("Username and/or password wrong");

                var res = await _signInManager.PasswordSignInAsync(user.UserName!, dto.Password, false, false);

                if (!res.Succeeded) return Unauthorized("Username and/or password wrong");

                var token = _tokenService.CreateToken(user);

                var refreshToken = _tokenService.CreateRefreshToken();

                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiryTime = _tokenService.GetRefreshTokenExpiry();
                await _userManager.UpdateAsync(user);

                return Ok(new TokenResponseDto
                {
                    AccessToken = token,
                    RefreshToken = refreshToken
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshRequestDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = await _userManager.Users
                .FirstOrDefaultAsync(u => u.RefreshToken == dto.RefreshToken);

            if (user == null) return Unauthorized("Invalid or expired refresh token");

            var token = _tokenService.CreateToken(user);

            var refreshToken = _tokenService.CreateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = _tokenService.GetRefreshTokenExpiry();
            await _userManager.UpdateAsync(user);

            return Ok(new TokenResponseDto
            {
                AccessToken = token,
                RefreshToken = refreshToken
            });
        }

        [AllowAnonymous]
        [HttpPost("google")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginDto dto)
        {
            var res = await _googleSvc.LoginAsync(dto.IdToken, HttpContext.Connection.RemoteIpAddress?.ToString());
            if (res is null) return Unauthorized("Invalid Google ID token");
            return Ok(res);
        }

        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest req)
        {
            // Trả message chung để không lộ thông tin
            var genericOk = Ok(new { message = "If that email exists, a new temporary password has been sent." });

            if (string.IsNullOrWhiteSpace(req.Email))
                return genericOk;

            var user = await _userManager.FindByEmailAsync(req.Email);
            // Có thể bạn muốn yêu cầu email phải confirmed mới cho reset:
            if (user is null /* || !(await _userManager.IsEmailConfirmedAsync(user)) */)
                return genericOk;

            // 1) Sinh password mới (đạt policy)
            var newPassword = PasswordGenerator.Generate(12);

            // 2) Tạo token reset NỘI BỘ rồi đổi password luôn
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetResult = await _userManager.ResetPasswordAsync(user, token, newPassword);
            if (!resetResult.Succeeded)
            {
                // Có thể do policy hoặc lockout. Không lộ chi tiết ra ngoài.
                return genericOk;
            }

            // 3) Revoke refresh tokens cũ (nếu bạn lưu trong user)
            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;
            await _userManager.UpdateAsync(user);

            // 4) Gửi email mật khẩu tạm
            var subject = "Your temporary password";
            var body = $@"
                <p>We have reset your password as requested.</p>
                <p><b>Temporary password:</b> <code>{newPassword}</code></p>
                <p>Please sign in and change your password immediately.</p>
                <hr />
                <p>Nếu bạn không yêu cầu, vui lòng bỏ qua email này hoặc liên hệ hỗ trợ.</p>";

            // LƯU Ý: From của bạn nên trùng Gmail (đã nói ở bước trước)
            await _emailSender.SendAsync(user.Email!, subject, body);

            // 5) Luôn trả về message chung
            return genericOk;
        }

        [HttpPost("change-password-simple")]
        [AllowAnonymous]
        public async Task<IActionResult> ChangePasswordSimple([FromBody] ChangePasswordSimpleDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = await _userManager.FindByNameAsync(dto.Username);
            if (user is null)
                return Unauthorized("Username and/or password is incorrect.");

            // Dùng SignInManager để áp dụng cơ chế lockout nếu đã cấu hình
            var check = await _signInManager.CheckPasswordSignInAsync(user, dto.OldPassword, lockoutOnFailure: true);
            if (!check.Succeeded)
                return Unauthorized("Username and/or password is incorrect.");

            // Đổi mật khẩu (Identity sẽ validate policy: độ dài, chữ hoa/thường/ký tự đặc biệt...)
            var result = await _userManager.ChangePasswordAsync(user, dto.OldPassword, dto.NewPassword);
            if (!result.Succeeded)
                return BadRequest(new { errors = result.Errors.Select(e => e.Description) });

            // Revoke refresh tokens cũ nếu bạn đang lưu trong user
            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;
            await _userManager.UpdateAsync(user);

            return Ok(new { message = "Password changed successfully." });
        }
    }
}