using Backend_FPTU_Internal_Event.BLL.DTOs;
using Backend_FPTU_Internal_Event.BLL.Interfaces;
using Backend_FPTU_Internal_Event.BLL.Models;
using Backend_FPTU_Internal_Event.DAL.Data;
using Backend_FPTU_Internal_Event.DAL.Entities;
using Backend_FPTU_Internal_Event.DAL.Interface;
using Backend_FPTU_Internal_Event.DAL.Repositories;
using Google.Apis.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;


namespace Backend_FPTU_Internal_Event.BLL.Services
{
    public class AuthService : IAuthService //
    {
        private readonly IUserRepository _userRepository;
        private readonly JwtSettings _jwtSettings;
        private readonly string _googleClientId;

        public AuthService(IUserRepository userRepository, IOptions<JwtSettings> jwtSettings, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _jwtSettings = jwtSettings.Value;
            _googleClientId = configuration["GoogleAuth:ClientId"] ?? throw new InvalidOperationException("Google ClientId not configured");
        }

        public LoginResponse? Login(LoginRequest request)
        {
            string hashedInput = HashPassword(request.Password);
            var user = _userRepository.Login(request.Email, hashedInput);


            if (user == null)
                return null;

            var token = GenerateJwtToken(user.UserId, user.Email, user.Role.RoleName);
            var expiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes);

            return new LoginResponse
            {
                UserId = user.UserId,
                UserName = user.UserName,
                Email = user.Email,
                RoleName = user.Role.RoleName,
                Token = token,
                ExpiresAt = expiresAt
            };
        }

        public async Task<LoginResponse?> GoogleLogin(GoogleLoginRequest request)
        {
            try
            {
                // Verify Google Token
                var payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken, new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { _googleClientId }
                });

                var email = payload.Email;

                // Logic kiểm tra theo yêu cầu
                if (email.EndsWith("@fpt.edu.vn"))
                {
                    // K18 - có email FPT, cho phép login ngay
                    var existingUser = _userRepository.GetUserByEmail(email);

                    if (existingUser == null)
                    {
                        // Tự động tạo user mới cho K18
                        var newUser = new User
                        {
                            Email = email,
                            UserName = payload.Name ?? email.Split('@')[0],
                            HashPassword = string.Empty, // Không cần password cho Google login
                            RoleId = 2 // Mặc định là Student role (cần điều chỉnh theo DB của bạn)
                        };

                        _userRepository.AddUser(newUser);
                        _userRepository.SaveChanges();

                        // Reload user để có đầy đủ thông tin Role
                        existingUser = _userRepository.GetUserByEmail(email);
                    }

                    return GenerateLoginResponse(existingUser);
                }
                else if (email.EndsWith("@gmail.com"))
                {
                    // K19 - phải check trong DB trước
                    var existingUser = _userRepository.GetUserByEmail(email);

                    if (existingUser == null)
                    {
                        // Email chưa có trong DB, không cho phép login
                        return null;
                    }

                    return GenerateLoginResponse(existingUser);
                }
                else
                {
                    // Email không hợp lệ (không phải @fpt.edu.vn hoặc @gmail.com)
                    return null;
                }
            }
            catch (Exception)
            {
                // Token không hợp lệ hoặc lỗi khác
                return null;
            }
        }

        private LoginResponse GenerateLoginResponse(User? user)
        {
            if (user == null || user.Role == null)
                return null;

            var token = GenerateJwtToken(user.UserId, user.Email, user.Role.RoleName);
            var expiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes);

            return new LoginResponse
            {
                UserId = user.UserId,
                UserName = user.UserName,
                Email = user.Email,
                RoleName = user.Role.RoleName,
                Token = token,
                ExpiresAt = expiresAt
            };
        }

        public string GenerateJwtToken(int userId, string email, string roleName)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim(ClaimTypes.Role, roleName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("userId", userId.ToString()),
                new Claim("roleName", roleName)
            };

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }
    }
}