using System.Net.Http.Json;
using System.Threading.Tasks;
using System;
using SharedLibrary.Response;
using System.Net;

namespace SharedLibrary.ServiceClients
{
    public class AuthServiceClient
    {
        private readonly HttpClient _httpClient;

        public AuthServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
            
            // Ensure the HttpClient uses secure HTTPS
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;
        }

        public async Task<ApiResponse<object>> ValidateTokenAsync(string token)
        {
            try
            {
                var request = new { Token = token };
                var response = await _httpClient.PostAsJsonAsync("/api/auth/validate", request);
                
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
                }
                
                return new ApiResponse<object> 
                { 
                    Success = false, 
                    Message = "Failed to validate token" 
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<object>
                {
                    Success = false,
                    Message = $"Error validating token: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<object>> GetUserInfoAsync(int userId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/auth/users/{userId}");
                
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
                }
                
                return new ApiResponse<object> 
                { 
                    Success = false, 
                    Message = "Failed to get user information" 
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<object>
                {
                    Success = false,
                    Message = $"Error getting user info: {ex.Message}"
                };
            }
        }
        
        public async Task<dynamic> GetUserByIdAsync(int userId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/auth/users/{userId}");
                
                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<dynamic>>();
                    return apiResponse.Data;
                }
                
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
} 
 