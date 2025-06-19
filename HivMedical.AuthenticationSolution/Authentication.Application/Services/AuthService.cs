using Authentication.Application.DTOs;
using Authentication.Application.Interfaces;
using Authentication.Domain.Entities;
using SharedLibrary.Jwt;
using SharedLibrary.Messaging;
using SharedLibrary.Messaging.Events;
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
        private readonly IEventBus _eventBus;

        public AuthService(IUnitOfWork unitOfWork, IJwtService jwtService, IEventBus eventBus)
        {
            _jwtService = jwtService;
            _uow = unitOfWork;
            _eventBus = eventBus;
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

        public async Task<ApiResponse<LogoutResponse>> LogoutAsync()
        {
           
            return new ApiResponse<LogoutResponse>
            {
                Success = true,
                Message = "Logout Successful",
                Data = new LogoutResponse
                {
                    LogoutTime = DateTime.UtcNow
                }
            };
        }

        public async Task<ApiResponse<RegisterResponse>> RegisterAsync(RegisterRequest request)
        {
          
            if (await _uow.Users.EmailExistsAsync(request.Email))
            {
                return new ApiResponse<RegisterResponse>
                {
                    Success = false,
                    Message = "Email đã được sử dụng"
                };
            }

           
            var user = new User
            {
                Email = request.Email,
                PasswordHash = Hash(request.Password),
                FullName = request.FullName,
                Phone = request.Phone,
                DateOfBirth = request.DateOfBirth,
                Gender = request.Gender,
                IsAnonymous = false,
                RoleId = 2, 
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

           
            var createdUser = await _uow.Users.GetUserWithRoleByEmailAsync(request.Email);

            
            try
            {
                var userRegisteredEvent = new UserRegisteredEvent(
                    createdUser.Id,
                    createdUser.Email,
                    createdUser.FullName ?? string.Empty,
                    createdUser.Role.Name
                );
                
                _eventBus.Publish(userRegisteredEvent);
            }
            catch (Exception ex)
            {           
                Console.WriteLine($"Failed to publish UserRegisteredEvent: {ex.Message}");
            }

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
