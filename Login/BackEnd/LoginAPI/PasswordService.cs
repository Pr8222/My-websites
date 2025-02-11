using Microsoft.AspNetCore.Identity;
using Models;

namespace LoginAPI
{
    public class PasswordService
    {
        private readonly PasswordHasher<User> _passwordHasher = new PasswordHasher<User>();

        public PasswordService()
        {
            _passwordHasher = new PasswordHasher<User>();
        }

        public string HashPassword(User user, string password)
        {
            return _passwordHasher.HashPassword(user, password);
        }
    }
}
