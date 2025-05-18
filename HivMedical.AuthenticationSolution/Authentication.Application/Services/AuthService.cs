using Authentication.Application.DTOs;
using Authentication.Application.Interfaces;
using SharedLibrary.Jwt;
using SharedLibrary.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Authentication.Application.Services
{
    public class AuthService
    {
        private readonly IUnitOfWork _uow;
        private readonly IJwtService _jwtService;

        public AuthService(IUnitOfWork unitOfWork, IJwtService jwtService)
        {
            _jwtService = jwtService;
            _uow = unitOfWork;
        }

        public async Task<ApiResponse<LoginResponse>> LoginAsync(LoginRequest request)
        {
            var user = await _uow.Users.GetUserWithRoleByEmailAsync(request.Email);
            if (user is null || user.PasswordHash != Hash(request.Password))
            {
                return new ApiResponse<LoginResponse> { Success = false, Message = "Invalid credentials" };

            }
            var token = _jwtService.GenerateToken(user.Id, user.Role.Name);
            return new ApiResponse<LoginResponse>
            {
                Success = true,
                Message = "Login Successful",
                Data = new()
                {
                    AccessToken = token,
                    FullName = user.FullName ?? "Anonymous",
                    Role = user.Role.Name
                }
            };
        }

        private string Hash(string input)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(input);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}
