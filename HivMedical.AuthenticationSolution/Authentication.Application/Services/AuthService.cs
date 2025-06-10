using Authentication.Application.DTOs;
using Authentication.Application.Interfaces;
using Authentication.Domain.Entities;
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

        public async Task<ApiResponse<RegisterResponse>> RegisterAsync(RegisterRequest request)
        {
            // Check if email already exists
            if (await _uow.Users.EmailExistsAsync(request.Email))
            {
                return new ApiResponse<RegisterResponse>
                {
                    Success = false,
                    Message = "Email đã được sử dụng"
                };
            }

            // Create new user with default role (assuming RoleId 2 is for regular users)
            var user = new User
            {
                Email = request.Email,
                PasswordHash = Hash(request.Password),
                FullName = request.FullName,
                Phone = request.Phone,
                DateOfBirth = request.DateOfBirth,
                Gender = request.Gender,
                IsAnonymous = false,
                RoleId = 2, // Assuming 2 is the default "User" role
                CreatedAt = DateTime.UtcNow
            };

            var result = await _uow.Users.CreateAsync(user);
            if (!result.Success)
            {
                return new ApiResponse<RegisterResponse>
                {
                    Success = false,
                    Message = "Đăng ký thất bại: " + result.Message
                };
            }

            await _uow.SaveChangesAsync();

            // Get user with role for response
            var createdUser = await _uow.Users.GetUserWithRoleByEmailAsync(request.Email);

            return new ApiResponse<RegisterResponse>
            {
                Success = true,
                Message = "Đăng ký thành công",
                Data = new RegisterResponse
                {
                    UserId = createdUser.Id,
                    Email = createdUser.Email,
                    FullName = createdUser.FullName,
                    Role = createdUser.Role.Name
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
