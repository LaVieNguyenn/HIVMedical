namespace SharedLibrary.Jwt
{
    public interface IJwtService
    {
        string GenerateToken(int userId, string role);
    }
}
