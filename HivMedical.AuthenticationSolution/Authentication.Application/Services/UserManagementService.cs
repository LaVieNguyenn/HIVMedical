using Authentication.Application.DTOs;
using Authentication.Application.Interfaces;
using Authentication.Domain.Entities;
using SharedLibrary.Response;
using System.Security.Cryptography;
using System.Text;

namespace Authentication.Application.Services
{
    public class UserManagementService
    {
        private readonly IUnitOfWork _uow;

        public UserManagementService(IUnitOfWork unitOfWork)
        {
            _uow = unitOfWork;
        }

        public async Task<ApiResponse<PagedUserListResponse>> GetUsersAsync(UserFilterRequest filter)
        {
            try
            {
                var result = await _uow.Users.GetUsersWithPaginationAsync(filter);
                return new ApiResponse<PagedUserListResponse>
                {
                    Success = true,
                    Data = result,
                    Message = "Users retrieved successfully"
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<PagedUserListResponse>
                {
                    Success = false,
                    Message = $"Error retrieving users: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<UserDetailsDto>> GetUserByIdAsync(int id)
        {
            try
            {
                var user = await _uow.Users.GetUserWithRoleByIdAsync(id);
                if (user == null)
                {
                    return new ApiResponse<UserDetailsDto>
                    {
                        Success = false,
                        Message = "User not found"
                    };
                }

                var userDto = new UserDetailsDto
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    FullName = user.FullName,
                    Email = user.Email,
                    Gender = user.Gender,
                    Phone = user.Phone,
                    IsAnonymous = user.IsAnonymous,
                    DateOfBirth = user.DateOfBirth,
                    Role = user.Role.Name,
                    RoleId = user.RoleId,
                    CreatedAt = user.CreatedAt,
                    UpdatedAt = user.UpdatedAt,
                    IsDeleted = user.IsDeleted,
                    CreatedBy = user.CreatedBy,
                    UpdatedBy = user.UpdatedBy
                };

                return new ApiResponse<UserDetailsDto>
                {
                    Success = true,
                    Data = userDto,
                    Message = "User retrieved successfully"
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<UserDetailsDto>
                {
                    Success = false,
                    Message = $"Error retrieving user: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<UserDetailsDto>> CreateUserAsync(CreateUserRequest request)
        {
            try
            {
                // Check if email already exists
                if (await _uow.Users.EmailExistsAsync(request.Email))
                {
                    return new ApiResponse<UserDetailsDto>
                    {
                        Success = false,
                        Message = "Email already exists"
                    };
                }

                // Validate role exists
                var role = await _uow.Roles.GetRoleByIdAsync(request.RoleId);
                if (role == null)
                {
                    return new ApiResponse<UserDetailsDto>
                    {
                        Success = false,
                        Message = "Invalid role specified"
                    };
                }

                var user = new User
                {
                    UserName = request.UserName,
                    Email = request.Email,
                    PasswordHash = Hash(request.Password),
                    FullName = request.FullName,
                    Phone = request.Phone,
                    DateOfBirth = request.DateOfBirth,
                    Gender = request.Gender,
                    IsAnonymous = request.IsAnonymous,
                    RoleId = request.RoleId,
                    CreatedAt = DateTime.UtcNow
                };

                var result = await _uow.Users.CreateAsync(user);
                if (!result.Success)
                {
                    return new ApiResponse<UserDetailsDto>
                    {
                        Success = false,
                        Message = result.Message
                    };
                }

                await _uow.SaveChangesAsync();

                // Get the created user with role
                var createdUser = await _uow.Users.GetUserWithRoleByEmailAsync(request.Email);
                var userDto = new UserDetailsDto
                {
                    Id = createdUser.Id,
                    UserName = createdUser.UserName,
                    FullName = createdUser.FullName,
                    Email = createdUser.Email,
                    Gender = createdUser.Gender,
                    Phone = createdUser.Phone,
                    IsAnonymous = createdUser.IsAnonymous,
                    DateOfBirth = createdUser.DateOfBirth,
                    Role = createdUser.Role.Name,
                    RoleId = createdUser.RoleId,
                    CreatedAt = createdUser.CreatedAt,
                    UpdatedAt = createdUser.UpdatedAt,
                    IsDeleted = createdUser.IsDeleted,
                    CreatedBy = createdUser.CreatedBy,
                    UpdatedBy = createdUser.UpdatedBy
                };

                return new ApiResponse<UserDetailsDto>
                {
                    Success = true,
                    Data = userDto,
                    Message = "User created successfully"
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<UserDetailsDto>
                {
                    Success = false,
                    Message = $"Error creating user: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<UserDetailsDto>> UpdateUserAsync(int id, UpdateUserRequest request)
        {
            try
            {
                var user = await _uow.Users.GetUserWithRoleByIdAsync(id);
                if (user == null)
                {
                    return new ApiResponse<UserDetailsDto>
                    {
                        Success = false,
                        Message = "User not found"
                    };
                }

                // Check if email is being changed and if it already exists for another user
                if (!string.IsNullOrEmpty(request.Email) && request.Email != user.Email)
                {
                    if (await _uow.Users.EmailExistsForOtherUserAsync(request.Email, id))
                    {
                        return new ApiResponse<UserDetailsDto>
                        {
                            Success = false,
                            Message = "Email already exists for another user"
                        };
                    }
                    user.Email = request.Email;
                }

                // Validate role if being changed
                if (request.RoleId.HasValue && request.RoleId.Value != user.RoleId)
                {
                    var role = await _uow.Roles.GetRoleByIdAsync(request.RoleId.Value);
                    if (role == null)
                    {
                        return new ApiResponse<UserDetailsDto>
                        {
                            Success = false,
                            Message = "Invalid role specified"
                        };
                    }
                    user.RoleId = request.RoleId.Value;
                }

                // Update other fields
                if (!string.IsNullOrEmpty(request.FullName))
                    user.FullName = request.FullName;

                if (!string.IsNullOrEmpty(request.UserName))
                    user.UserName = request.UserName;

                if (!string.IsNullOrEmpty(request.Phone))
                    user.Phone = request.Phone;

                if (request.DateOfBirth.HasValue)
                    user.DateOfBirth = request.DateOfBirth;

                if (request.Gender.HasValue)
                    user.Gender = request.Gender.Value;

                if (request.IsAnonymous.HasValue)
                    user.IsAnonymous = request.IsAnonymous.Value;

                // Update password if provided
                if (!string.IsNullOrEmpty(request.NewPassword))
                    user.PasswordHash = Hash(request.NewPassword);

                user.UpdatedAt = DateTime.UtcNow;

                var result = await _uow.Users.UpdateAsync(user);
                if (!result.Success)
                {
                    return new ApiResponse<UserDetailsDto>
                    {
                        Success = false,
                        Message = result.Message
                    };
                }

                await _uow.SaveChangesAsync();

                // Get updated user with role
                var updatedUser = await _uow.Users.GetUserWithRoleByIdAsync(id);
                var userDto = new UserDetailsDto
                {
                    Id = updatedUser.Id,
                    UserName = updatedUser.UserName,
                    FullName = updatedUser.FullName,
                    Email = updatedUser.Email,
                    Gender = updatedUser.Gender,
                    Phone = updatedUser.Phone,
                    IsAnonymous = updatedUser.IsAnonymous,
                    DateOfBirth = updatedUser.DateOfBirth,
                    Role = updatedUser.Role.Name,
                    RoleId = updatedUser.RoleId,
                    CreatedAt = updatedUser.CreatedAt,
                    UpdatedAt = updatedUser.UpdatedAt,
                    IsDeleted = updatedUser.IsDeleted,
                    CreatedBy = updatedUser.CreatedBy,
                    UpdatedBy = updatedUser.UpdatedBy
                };

                return new ApiResponse<UserDetailsDto>
                {
                    Success = true,
                    Data = userDto,
                    Message = "User updated successfully"
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<UserDetailsDto>
                {
                    Success = false,
                    Message = $"Error updating user: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<bool>> DeleteUserAsync(int id)
        {
            try
            {
                var result = await _uow.Users.SoftDeleteAsync(id);
                if (!result.Success)
                {
                    return result;
                }

                await _uow.SaveChangesAsync();
                return new ApiResponse<bool>
                {
                    Success = true,
                    Data = true,
                    Message = "User deleted successfully"
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = $"Error deleting user: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<bool>> RestoreUserAsync(int id)
        {
            try
            {
                var result = await _uow.Users.RestoreUserAsync(id);
                if (!result.Success)
                {
                    return result;
                }

                await _uow.SaveChangesAsync();
                return new ApiResponse<bool>
                {
                    Success = true,
                    Data = true,
                    Message = "User restored successfully"
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = $"Error restoring user: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<List<RoleDto>>> GetRolesAsync()
        {
            try
            {
                var roles = await _uow.Roles.GetAllRolesAsync();
                var roleDtos = roles.Select(r => new RoleDto
                {
                    Id = r.Id,
                    Name = r.Name,
                    CreatedAt = r.CreatedAt
                }).ToList();

                return new ApiResponse<List<RoleDto>>
                {
                    Success = true,
                    Data = roleDtos,
                    Message = "Roles retrieved successfully"
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<RoleDto>>
                {
                    Success = false,
                    Message = $"Error retrieving roles: {ex.Message}"
                };
            }
        }

        private string Hash(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }
}
