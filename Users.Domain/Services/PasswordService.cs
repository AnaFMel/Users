namespace Users.Domain.Services
{
    public class PasswordService
    {
        public string CreateHash(string password) => BCrypt.Net.BCrypt.EnhancedHashPassword(password, workFactor: 12);
        public bool VerifyPassword(string password, string hash) => BCrypt.Net.BCrypt.EnhancedVerify(password, hash);
    }
}
