using Data;
using Microsoft.EntityFrameworkCore;

namespace LoginAPI.Services
{
    public class AccessControlService
    {
        private readonly UserContext _context;

        public AccessControlService(UserContext context)
        {
            _context = context;
        }

        public async Task<bool> UserHasKeyAsync(string userId, string keyName)
        {
            var roleKeyQuery = from ru in _context.RoleUsers
                               join rk in _context.RoleKeys on ru.RoleId equals rk.RoleId
                               join k in _context.Keys on rk.KeyId equals k.Id
                               where ru.UserId == userId && k.KeyName == keyName
                               select k;

            var userKeyQuery = from uk in _context.UserExtraKeys
                               join k in _context.Keys on uk.KeyId equals k.Id
                               where uk.UserId == userId && k.KeyName == keyName
                               select k;

            return await roleKeyQuery.AnyAsync() || await userKeyQuery.AnyAsync();
        }
    }

