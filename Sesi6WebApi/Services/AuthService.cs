using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Sesi6WebApi.Data;
using Sesi6WebApi.DTO.Auth;
using Sesi6WebApi.Entities;
using Sesi6WebApi.Exceptions;
using Sesi6WebApi.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Sesi6WebApi.Services
{
    public sealed class AuthService(
    AppDbContext db,
    IConfiguration configuration,
    IPasswordHasher<AppUser> passwordHasher) : IAuthService
    {
        public async Task<LoginResponse> RegisterAsync(RegisterRequest request)
        {
            var email = request.Email.Trim().ToLowerInvariant();

            if (await db.Users.AnyAsync(x => x.Email == email))
                throw new BusinessException("Email sudah digunakan.");

            var user = new AppUser
            {
                FullName = request.FullName.Trim(),
                Email = email,
                Role = "User",
                CreatedAt = DateTime.UtcNow
            };

            user.PasswordHash = passwordHasher.HashPassword(user, request.Password);

            db.Users.Add(user);
            await db.SaveChangesAsync();

            return CreateResponse(user);
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            var email = request.Email.Trim().ToLowerInvariant();
            var user = await db.Users.SingleOrDefaultAsync(x => x.Email == email);

            if (user is null)
                throw new UnauthorizedException("Email atau password salah.");

            var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);

            if (result == PasswordVerificationResult.Failed)
                throw new UnauthorizedException("Email atau password salah.");

            if (result == PasswordVerificationResult.SuccessRehashNeeded)
            {
                user.PasswordHash = passwordHasher.HashPassword(user, request.Password);
                await db.SaveChangesAsync();
            }

            return CreateResponse(user);
        }

        private LoginResponse CreateResponse(AppUser user)
        {
            var expireMinutesValue = configuration["Jwt:ExpireMinutes"];
            var expireMinutes = int.TryParse(expireMinutesValue, out var m) ? m : 60;
            var expiresAt = DateTime.UtcNow.AddMinutes(expireMinutes);

            var token = GenerateJwtToken(user, expiresAt);

            return new LoginResponse
            {
                Token = token,
                ExpiresAt = expiresAt,
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role
            };
        }

        private string GenerateJwtToken(AppUser user, DateTime expiresAt)
        {
            var key = configuration["Jwt:Key"]
                ?? throw new InvalidOperationException("Jwt:Key belum dikonfigurasi.");

            if (key.Length < 32)
                throw new InvalidOperationException("Jwt:Key minimal 32 karakter.");

            var claims = new[]
            {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: configuration["Jwt:Issuer"],
                audience: configuration["Jwt:Audience"],
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: expiresAt,
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
